using SCDataSync.Memory;
using System.Runtime.InteropServices;
using System.Text;
using SCDataSync.Memory.Extensions;

namespace SCDataSync.Communication.IpcProtocol
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct PingStruct
    {
        internal byte updatePing;

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
    internal class PingProtocol
    {
        internal PingProtocol(JhMemory j, ulong baseAddress)
        {
            this._j = j;
            this._baseAddress = baseAddress;
        }

        private readonly JhMemory _j;
        private readonly ulong _baseAddress;
        private bool WritePingStruct(byte updatePing)
        {
            var pingStruct = new PingStruct
            {
                updatePing = updatePing
            };
            return _j.Write(_baseAddress, pingStruct.AsByteSpan());
        }

        internal bool SendUpdatePing()
        {
            return WritePingStruct((byte)1);
        }
    }

}
