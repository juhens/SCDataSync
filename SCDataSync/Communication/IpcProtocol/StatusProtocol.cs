using SCDataSync.Memory;
using System.Runtime.InteropServices;
using System.Text;
using SCDataSync.Memory.Extensions;

namespace SCDataSync.Communication.IpcProtocol
{
    internal enum Response : ushort
    {
        None = 0,

        Done = 1,
        Pending = 2,
        Complete = 3,

        DuplicateConnect = 201,
        DuplicateDisconnect = 202,
        DuplicateTimeout = 203,
        DuplicateSendComplete = 204,

        ChecksumMissing = 301,
        ChecksumMissMatch = 302,
        DataIndexMissing = 303,
        DataLengthMissing = 304,

        InvalidRequest = 401,
        OutOfIndex = 402,

    }
    internal enum ConnectionStatus : ushort
    {
        None = 0,
        Connect = 1,
        Disconnect = 2,
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct StatusStruct
    {
        internal ConnectionStatus connectionStatus;
        internal Response response;

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var b in this.AsByteSpan())
            {
                sb.Append($"{b:X2} ");
            }
            return sb.ToString();
        }
    }
    internal class StatusProtocol
    {
        internal StatusProtocol(JhMemory j, ulong baseAddress)
        {
            this._j = j;
            this._baseAddress = baseAddress;
        }

        private readonly JhMemory _j;
        private readonly ulong _baseAddress;

        private bool ReadStatusStruct(ref StatusStruct statusStruct)
        {
            var span = statusStruct.AsByteSpan();
            return _j.Read(_baseAddress, ref span);
        }

        internal bool ReceiveStatusInformation(ref StatusStruct statusStruct)
        {
            return ReadStatusStruct(ref statusStruct);
        }
    }
}
