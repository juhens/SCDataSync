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

        private Span<byte> GetByteSpan()
        {
            var thisStructSpan = MemoryMarshal.CreateSpan(ref this, 1);
            return MemoryMarshal.Cast<LockStruct, byte>(thisStructSpan);
        }
        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var b in GetByteSpan())
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
            return _j.Write(_baseAddress, lockStruct.AsByteSpan());
        }

        private bool ReadLockStruct(ref LockStruct lockStruct)
        {
            return _j.Read(_baseAddress, lockStruct.AsByteSpan());
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
