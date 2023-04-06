using SCDataSync.Memory;
using System.Runtime.InteropServices;
using System.Text;

namespace SCDataSync.Communication.IpcProtocol
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct HeaderStruct
    {
        private readonly long _dummy0;
        private readonly long _dummy1;
        private readonly long _dummy2;
        internal uint version;
        internal byte userCp;
        private readonly byte _dummy3;
        private readonly byte _dummy4;
        internal byte encryptedFlag;
        internal uint seed;
        internal uint headerRegionSize;
        internal uint commMessageLockRegionSize;
        internal uint commMessageRegionSize;
        internal uint commMsqcLockRegionSize;
        internal uint commMsqcRegionSize;
        internal uint commPingRegionSize;
        internal uint commStatusRegionSize;
        internal uint dataRegionSize;

        private Span<byte> GetByteSpan()
        {
            var thisStructSpan = MemoryMarshal.CreateSpan(ref this, 1);
            return MemoryMarshal.Cast<HeaderStruct, byte>(thisStructSpan);
        }

        internal readonly string GetNameAndVersion()
        {
            return $"SCDataSync {version >> 24}.{(version >> 16) & 0xFF}.{(version >> 8) & 0xFF}.{version & 0xFF}";
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            const int length = -26;
            sb.AppendLine("SCDataSync");
            sb.AppendLine($"{"Version",length}{version >> 24}.{(version >> 16) & 0xFF}.{(version >> 8) & 0xFF}.{version & 0xFF}");
            sb.AppendLine($"{"UserCp",length}{userCp}");
            sb.AppendLine($"{"EncryptedFlag",length}{(encryptedFlag == 1 ? true : false)}");
            sb.AppendLine($"{"Seed",length}{seed}");
            sb.AppendLine($"{"HeaderRegionSize",length}0x{headerRegionSize:X2}");
            sb.AppendLine($"{"CommMessageLockRegionSize",length}0x{commMessageLockRegionSize:X2}");
            sb.AppendLine($"{"CommMessageRegionSize",length}0x{commMessageRegionSize:X2}");
            sb.AppendLine($"{"CommMsqcLockRegionSize",length}0x{commMsqcLockRegionSize:X2}");
            sb.AppendLine($"{"CommMsqcRegionSize",length}0x{commMsqcRegionSize:X2}");
            sb.AppendLine($"{"CommPingRegionSize",length}0x{commPingRegionSize:X2}");
            sb.AppendLine($"{"CommStatusRegionSize",length}0x{commStatusRegionSize:X2}");
            sb.AppendLine($"{"DataRegionSize",length}0x{dataRegionSize:X2}");
            return sb.ToString();
        }

        internal string GetMagicNumberString()
        {
            var sliceSpan = GetByteSpan()[..24];
            return sliceSpan.ToString();
        }
    }
    internal class HeaderProtocol
    {
        internal HeaderProtocol(JhMemory j, ulong baseAddress)
        {
            this._j = j;
            this._baseAddress = baseAddress;
        }

        private readonly JhMemory _j;
        private readonly ulong _baseAddress;

        private bool ReadHeaderStruct(ref HeaderStruct headerStruct)
        {
            return _j.Read(_baseAddress, ref headerStruct);
        }

        internal bool ReceiveHeaderInformation(ref HeaderStruct headerStruct)
        {
            return ReadHeaderStruct(ref headerStruct);
        }
    }
}
