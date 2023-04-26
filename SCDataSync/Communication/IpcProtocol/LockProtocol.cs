using SCDataSync.Memory;
using System.Runtime.InteropServices;
using System.Text;
using SCDataSync.Memory.Extensions;

namespace SCDataSync.Communication.IpcProtocol
{
    internal enum LockStatus : uint
    {
        Unlock = 0,
        Lock = 0xFFFFFFFF,
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct LockStruct
    {
        internal LockStatus lockStatus;

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
    internal class LockProtocol
    {
        internal LockProtocol(JhMemory j, ulong baseAddress)
        {
            this._j = j;
            this._baseAddress = baseAddress;
        }

        private readonly JhMemory _j;
        private readonly ulong _baseAddress;
        private bool WriteLockStruct(LockStatus lockStatus)
        {
            var lockStruct = new LockStruct
            {
                lockStatus = lockStatus
            };
            var span = lockStruct.AsReadOnlyByteSpan();
            return _j.Write(_baseAddress, span);
        }

        private bool ReadLockStruct(ref LockStruct lockStruct)
        {
            var span = lockStruct.AsByteSpan();
            return _j.Read(_baseAddress, span);
        }

        internal bool ReceiveLockState(ref LockStruct lockStruct)
        {
            return ReadLockStruct(ref lockStruct);
        }
        internal bool SendUnlock()
        {
            return WriteLockStruct(LockStatus.Unlock);
        }
    }

}
