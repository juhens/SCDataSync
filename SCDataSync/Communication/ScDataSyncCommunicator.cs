using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using SCDataSync.Communication.IpcProtocol;
using SCDataSync.Memory;
using SCDataSync.Memory.Extensions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SCDataSync.Communication
{
    internal class ScDataSyncCommunicator
    {
        private readonly LockProtocol _messageLockProtocol;
        private readonly MessageProtocol _messageProtocol;
        private readonly LockProtocol _msqcLockProtocol;
        private readonly MsqcProtocol _msqcProtocol;
        private readonly PingProtocol _pingProtocol;
        private readonly StatusProtocol _statusProtocol;
        private readonly DataProtocol _dataProtocol;
        private readonly HeaderStruct _headerInformation;

        private readonly Task _workerUpdatePing;
        private bool _workerUpdatePingState = true;
        private const int BufferSize = 32;

        internal readonly uint UserRegionSize;
        internal ScDataSyncCommunicator()
        {
            //check StarCraft running
            var sc = Process.GetProcessesByName("StarCraft");
            if (sc.Length == 0)
            {
                throw new Exception("Cannot find StarCraft");
            }

            //get shared memory base address
            JhMemory j = new(sc[0]);
            var magicNumber = "SCDATASYNC_STARTING_ADDR"u8;
            ulong baseAddress = j.Scan(magicNumber) ?? throw new Exception("Cannot find SCDataSync shared memory");

            //read header
            var headerProtocol = new HeaderProtocol(j, baseAddress);
            _headerInformation = new HeaderStruct();

            if (!headerProtocol.ReceiveHeaderInformation(ref _headerInformation))
            {
                throw new Exception("Cannot read header information");
            }

            //calc region base address
            ulong messageLockBaseAddress = baseAddress + _headerInformation.headerRegionSize;
            ulong messageBaseAddress = messageLockBaseAddress + _headerInformation.commMessageLockRegionSize;
            ulong msqcLockBaseAddress = messageBaseAddress + _headerInformation.commMessageRegionSize;
            ulong msqcBaseAddress = msqcLockBaseAddress + _headerInformation.commMsqcLockRegionSize;
            ulong pingBaseAddress = msqcBaseAddress + _headerInformation.commMsqcRegionSize;
            ulong statusBaseAddress = pingBaseAddress + _headerInformation.commPingRegionSize;
            ulong dataBaseAddress = statusBaseAddress + _headerInformation.commStatusRegionSize;



            UserRegionSize = _headerInformation.dataRegionSize / 8;


            //create protocol instance
            _messageLockProtocol = new LockProtocol(j, messageLockBaseAddress);
            _messageProtocol = new MessageProtocol(j, messageBaseAddress);
            _msqcLockProtocol = new LockProtocol(j, msqcLockBaseAddress);
            _msqcProtocol = new MsqcProtocol(j, msqcBaseAddress);
            _pingProtocol = new PingProtocol(j, pingBaseAddress);
            _statusProtocol = new StatusProtocol(j, statusBaseAddress);
            _dataProtocol = new DataProtocol(j, dataBaseAddress, UserRegionSize, _headerInformation.userCp);

            //create task
            _workerUpdatePing = new Task(UpdatePing);
        }

        
        //------------------------------------------------------------------------------
        internal void PrintInformation()
        {
            //print information
            Console.WriteLine(_headerInformation);
            TrySendMessageWithLockControl(MessageType.Normal, $"{_headerInformation.GetNameAndVersion()}");
        }
        internal void Connect()
        {
            bool isConnect;
            Console.WriteLine("send connect request and wait for response");
            TrySendMessageWithLockControl(MessageType.Normal, $"연결 요청후 응답 대기중");
            do
            {
                TrySendRequestWithLockControl(Request.Connect);
                isConnect = WaitForStatus(ConnectionStatus.Connect);
            }
            while (isConnect == false);
        }
        internal void SendData(ReadOnlySpan<byte> dataByteSpan, int startIndex)
        {
            if (dataByteSpan.Length > UserRegionSize)
            {
                throw new Exception("file size is larger than the maximum allowed size for users");
            }
            Console.WriteLine("send data request");
            TrySendMessageWithLockControl(MessageType.Normal, $"데이터 전송 시작");
            TrySendDataWithLockControl(dataByteSpan, startIndex);
            Console.WriteLine();
        }
        internal void UpdatePingStart()
        {
            //update ping
            _workerUpdatePing.Start();
            _workerUpdatePingState = true;

            //wait for update ping to start
            int count = 0;
            while (_workerUpdatePing.Status != TaskStatus.Running)
            {
                count++;
                Thread.Sleep(100);
                if (count > 5)
                    throw new Exception("failed update ping task start");
            }
        }
        internal void WaitForPendingResponse()
        {
            Console.WriteLine("send pending request and wait for response");
            TrySendMessageWithLockControl(MessageType.Normal, $"작업 대기 요청후 응답 대기중");
            do
            {
                TrySendRequestWithLockControl(Request.Pending);
            }
            while (WaitForResponse(Response.Pending) == false);
        }
        internal void CheckDataValidAndResend(ReadOnlySpan<byte> data, Span<byte> buffer)
        {
            var tryCount = 0;
            while (true)
            {
                tryCount++;

                Console.WriteLine($"check data validation {tryCount}");
                TrySendMessageWithLockControl(MessageType.Normal, $"데이터 검증 {tryCount}");

                //read sent data
                const uint startIndex = 0;
                _dataProtocol.ReceiveData(buffer, startIndex);

                //make errorIndexes
                var errorIndexes = new List<int>();
                for (var i = 0; i < data.Length; i += BufferSize)
                {
                    var length = Math.Min(data.Length - i, BufferSize);
                    var dataByteSpan = data.Slice(i, length);
                    var dumpDataBufferByteSpan = buffer.Slice(i, length);

                    if (!dataByteSpan.SequenceEqual(dumpDataBufferByteSpan))
                    {
                        errorIndexes.Add(i);
                    }
                }

                //resend data
                for (var i = 0; i < errorIndexes.Count; i++)
                {
                    var idx = errorIndexes[i];
                    var length = Math.Min(data.Length - idx, BufferSize);
                    var dataByteSpan = data.Slice(idx, length);

                    TrySendDataWithLockControl(dataByteSpan, idx);
                    TrySendMessageWithLockControl(MessageType.Normal, $"재전송 {i + 1} / {errorIndexes.Count}");
                    Console.Write($"\rresend {i + 1} / {errorIndexes.Count}");
                }

                if (errorIndexes.Count > 0)
                {
                    Console.WriteLine();
                    WaitForPendingResponse();
                }
                else
                    break;
            }
        }  
        internal void WaitForCompleteResponse()
        {
            Console.WriteLine("send complete request and wait for response");
            TrySendMessageWithLockControl(MessageType.Normal, $"완료 요청후 응답 대기중");
            do
            {
                TrySendRequestWithLockControl(Request.Complete);
            }
            while (WaitForResponse(Response.Complete) == false);
        }
        internal void UpdatePingStop()
        {
            _workerUpdatePingState = false;
            //wait for update ping to start
            int count = 0;
            while (_workerUpdatePing.Status != TaskStatus.RanToCompletion)
            {
                count++;
                Thread.Sleep(100);
                if (count > 5)
                    throw new Exception("failed update ping task stop");
            }
        }
        //-----------------------------------------------------------------------------


        private void TrySendDataWithLockControl(ReadOnlySpan<byte> dataByteSpan, int startIndex)
        {
            if (!WaitForStatus(ConnectionStatus.Connect))
            {
                throw new Exception($"connection status is not valid");
            }
            for (var i = 0; i < dataByteSpan.Length; i += BufferSize)
            {
                var sliceLength = Math.Min(dataByteSpan.Length - i, BufferSize);
                var sliceSpan = dataByteSpan.Slice(i, sliceLength);

                if (!WaitForLockRelease(_msqcLockProtocol))
                {
                    throw new Exception($"failed to acquire lock for {_msqcLockProtocol.GetType().Name}");
                }

                if (!_msqcProtocol.SendData(sliceSpan, (uint)(startIndex + i)))
                {
                    throw new Exception($"failed to send data using {_msqcProtocol.GetType().Name}");
                }

                if (!_msqcLockProtocol.SendUnlock())
                {
                    throw new Exception($"failed to release lock using {_msqcLockProtocol.GetType().Name}");
                }
                var currentLength = (i + sliceLength);
                PrintProgress(dataByteSpan.Length, currentLength, sliceLength);
            }
        }
        private void PrintProgress(int totalLength, int currentLength, int sliceLength)
        {
            if (currentLength % 512 == 0 || sliceLength < BufferSize)
            {
                TrySendMessageWithLockControl(MessageType.Normal, $"{currentLength:N0} bytes / {totalLength:N0} bytes");
                Console.Write($"\r{currentLength:N0} bytes / {totalLength:N0} bytes");
            }
        }

        private void TrySendRequestWithLockControl(Request request)
        {
            if (!WaitForLockRelease(_msqcLockProtocol))
            {
                throw new Exception($"failed to acquire lock for {_msqcLockProtocol.GetType().Name}");
            }

            if (!_msqcProtocol.SendRequest(request))
            {
                throw new Exception($"failed to send request using {_msqcProtocol.GetType().Name}");
            }

            if (!_msqcLockProtocol.SendUnlock())
            {
                throw new Exception($"failed to release lock using {_msqcLockProtocol.GetType().Name}");
            }
        }
        private void TrySendMessageWithLockControl(MessageType messageType, string str)
        {
            if (!WaitForLockRelease(_messageLockProtocol))
            {
                throw new Exception($"failed to acquire lock for {_messageLockProtocol.GetType().Name}");
            }

            if (!_messageProtocol.SendMessage(messageType, str))
            {
                throw new Exception($"failed to send message using {_messageProtocol.GetType().Name}");
            }

            if (!_messageLockProtocol.SendUnlock())
            {
                throw new Exception($"failed to release lock using {_messageLockProtocol.GetType().Name}");
            }
        }

        private bool WaitForStatus(ConnectionStatus states)
        {
            bool ConditionFunc(StatusProtocol protocol)
            {
                var statusStruct = new StatusStruct();
                return protocol.ReceiveStatusInformation(ref statusStruct) && statusStruct.connectionStatus == states;
            }

            return WaitForCondition(_statusProtocol, ConditionFunc);
        }
        private bool WaitForResponse(Response response)
        {
            bool ConditionFunc(StatusProtocol protocol)
            {
                var statusStruct = new StatusStruct();
                return protocol.ReceiveStatusInformation(ref statusStruct) && statusStruct.response == response;
            }

            return WaitForCondition(_statusProtocol, ConditionFunc);
        }
        private static bool WaitForLockRelease(LockProtocol lockProtocol)
        {
            bool ConditionFunc(LockProtocol protocol)
            {
                var lockStruct = new LockStruct();
                return protocol.ReceiveLockState(ref lockStruct) && lockStruct.lockStatus == LockStatus.Lock;
            }
            return WaitForCondition(lockProtocol, ConditionFunc);
        }
        private static bool WaitForCondition<T>(T protocol, Func<T, bool> conditionFunc) where T : class
        {
            const int tryMaxCount = 500;

            for (var tryCount = 0; tryCount < tryMaxCount; tryCount++)
            {
                if (conditionFunc(protocol))
                {
                    return true;
                }
                Thread.Sleep(5);
            }
            return false;
        }

        private void UpdatePing()
        {
            Console.WriteLine("update ping start");
            while (_workerUpdatePingState)
            {
                _pingProtocol.SendUpdatePing();
                Thread.Sleep(100);
            }
            Console.WriteLine("update ping stop");
        }
    }
}
