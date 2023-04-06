﻿using System.Runtime.InteropServices;
using System.Text;
using SCDataSync.Memory;

namespace SCDataSync.Communication.IpcProtocol
{
    internal enum Request : ushort
    {
        None = 0,
        Pending = 1,
        Complete = 2,

        Connect = 101,
        Disconnect = 102,
        Timeout = 103,

        Send = 201,
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct MsqcStruct
    {
        internal Request request;
        private byte checksum;
        internal byte dataLength;
        internal uint dataIndex;
        private readonly long dummy0;
        private readonly long dummy1;
        private readonly long dummy2;
        private readonly long dummy3;

        private Span<byte> GetByteSpan()
        {
            var thisStructSpan = MemoryMarshal.CreateSpan(ref this, 1);
            return MemoryMarshal.Cast<MsqcStruct, byte>(thisStructSpan);
        }
        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (byte b in GetByteSpan())
            {
                sb.Append($"{b:X2} ");
            }
            return sb.ToString();
        }

        internal void SetData(ReadOnlySpan<byte> byteSpan)
        {
            // 8 means the sum of size request, checksum, dataLength, dataIndex
            var sliceSpan = GetByteSpan()[8..];
            byteSpan.CopyTo(sliceSpan);
        }
        internal void GenerateChecksum()
        {
            var structSpan = MemoryMarshal.CreateSpan(ref this, 1);
            var ushortSpan = MemoryMarshal.Cast<MsqcStruct, ushort>(structSpan);
            //2 means the sum of size request, checksum, dataLength
            var sliceSpan = ushortSpan[2..];
            uint sum = 0;
            foreach (ushort s in sliceSpan)
            {
                sum += s;
            }
            sum += (ushort)request;
            sum += dataLength;
            checksum = (byte)(0xFF - (byte)sum);
        }
    }
    internal class MsqcProtocol
    {
        internal MsqcProtocol(JhMemory j, ulong baseAddress)
        {
            this._j = j;
            this._baseAddress = baseAddress;
        }

        private readonly JhMemory _j;
        private readonly ulong _baseAddress;
        private const uint MaxDataLength = 32;
        private bool WriteMsqcStruct(Request request, uint dataIndex, ReadOnlySpan<byte> dataSpan)
        {
            if (dataSpan.Length > MaxDataLength)
            {
                throw new ArgumentException($"maximum data length is {MaxDataLength} bytes. (input : {dataSpan.Length} bytes)");
            }
            var msqcStruct = new MsqcStruct
            {
                request = request,
                dataIndex = dataIndex,
                dataLength = (byte)dataSpan.Length,
            };
            if (!dataSpan.IsEmpty)
            {
                msqcStruct.SetData(dataSpan);
            }
                
            msqcStruct.GenerateChecksum();
            return _j.Write(_baseAddress, msqcStruct);
        }

        internal bool SendRequest(Request request)
        {
            return WriteMsqcStruct(request, 0, ReadOnlySpan<byte>.Empty);
        }
        internal bool SendData<T>(T value, uint dataIndex) where T : unmanaged
        {
            ReadOnlySpan<T> valueSpan = MemoryMarshal.CreateReadOnlySpan(ref value, 1);
            ReadOnlySpan<byte> valueByteSpan = MemoryMarshal.Cast<T, byte>(valueSpan);
            return WriteMsqcStruct(Request.Send, dataIndex, valueByteSpan);
        }
        internal bool SendData<T>(T[] value, uint dataIndex) where T : unmanaged
        {
            ReadOnlySpan<byte> valueByteSpan = MemoryMarshal.Cast<T, byte>(value);
            return WriteMsqcStruct(Request.Send, dataIndex, valueByteSpan);
        }
        internal bool SendData(ReadOnlySpan<byte> valueByteSpan, uint dataIndex)
        {
            return WriteMsqcStruct(Request.Send, dataIndex, valueByteSpan);
        }
    }


}