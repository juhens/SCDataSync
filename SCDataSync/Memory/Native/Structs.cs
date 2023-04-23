using System.Runtime.InteropServices;

namespace SCDataSync.Memory.Native
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ModuleInformation
    {
        public readonly UIntPtr lpBaseOfDll;
        public readonly uint SizeOfImage;
        public readonly UIntPtr EntryPoint;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct MemoryBasicInformation
    {
        public ulong BaseAddress;
        public ulong AllocationBase;
        public uint AllocationProtect;
        public uint __alignment1;
        public ulong RegionSize;
        public uint State;
        public uint Protect;
        public uint Type;
        public uint __alignment2;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ThreadBasicInformation
    {
        public int ExitStatus;
        public ulong TebBaseAddress;
        public ClientId ClientId;
        public ulong AffinityMask; // x86 == 4, x64 == 8
        public int Priority;
        public int BasePriority;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct ClientId
    {
        nint UniqueProcess;
        nint UniqueThread;
    }


    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SystemInformation
    {
        public ushort processorArchitecture;
        //ushort reserved;
        public uint pageSize;
        public nint minimumApplicationAddress;
        public nint maximumApplicationAddress;
        public nint activeProcessorMask;
        public uint numberOfProcessors;
        public uint processorType;
        public uint allocationGranularity;
        public ushort processorLevel;
        public ushort processorRevision;
    }
}
