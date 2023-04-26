using System.Runtime.InteropServices;
using System.Text;
using SCDataSync.Memory;
using SCDataSync.Memory.Extensions;

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
        private readonly _32 dataBuffer;

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (byte b in this.AsByteSpan())
            {
                sb.Append($"{b:X2} ");
            }
            return sb.ToString();
        }

        internal void SetData(ReadOnlySpan<byte> byteSpan)
        {
            // 8 means the sum of size request, checksum, dataLength, dataIndex
            var sliceSpan = this.AsByteSpan()[8..];
            byteSpan.CopyTo(sliceSpan);
        }
        internal void GenerateChecksum()
        {
            //2 means the sum of size request, checksum, dataLength
            var ushortSpan = this.AsCastSpan<MsqcStruct, ushort>()[2..];
            uint sum = 0;
            foreach (var s in ushortSpan)
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
            var span = msqcStruct.AsReadOnlyByteSpan();
            return _j.Write(_baseAddress, span);
        }

        internal bool SendRequest(Request request)
        {
            var emptySpan = ReadOnlySpan<byte>.Empty;
            return WriteMsqcStruct(request, 0, emptySpan);
        }
        internal bool SendData(ReadOnlySpan<byte> valueByteSpan, uint dataIndex)
        {
            return WriteMsqcStruct(Request.Send, dataIndex, valueByteSpan);
        }
    }


}
