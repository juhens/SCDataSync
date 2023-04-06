using System.Runtime.InteropServices;

namespace SCDataSync.Memory.Native
{
    public static class Kernel32
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool ReadProcessMemory(nint hProcess, ulong lpBaseAddress, ref byte lpBuffer, nuint nSize, out nuint lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteProcessMemory(nint hProcess, ulong lpBaseAddress, ref byte lpBuffer, nuint nSize, out nuint lpNumberOfBytesWritten);

        [DllImport("kernel32.dll")]
        public static extern int IsWow64Process(nint hProcess, out bool bWow64Process);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern nint OpenThread(Enums.ThreadAccess dwDesiredAccess, bool bInherihThread, int dwThreadId);

        [DllImport("ntdll.dll", SetLastError = true, EntryPoint = "NtQueryInformationThread")]
        public static extern int NtQueryInformationThread(nint pHandle, Enums._THREAD_INFORMATION_CLASS infoClass, ref Structs.ThreadBasicInformation instance, int sizeOfInstance, out int length);

        [DllImport("kernel32.dll")]
        public static extern void GetSystemInfo(out Structs.SystemInformation lpSystemInfo);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int VirtualQueryEx(nint hProcess, ulong lpAddress, out Structs.MemoryBasicinformation lpBuffer, uint dwLength);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern ulong VirtualAlloc([In] ulong lpAddress, nuint dwSize, Enums.MEM_ALLOCATION_TYPE flAllocationType, Enums.MEM_PROTECTION flProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool VirtualFree([In] ulong lpAddress, nuint dwSize, Enums.MEM_ALLOCATION_TYPE dwFreeType);

        [DllImport("kernel32.dll", ExactSpelling = true)]
        public static extern nint GetConsoleWindow();

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern nint GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetConsoleMode(nint hConsoleHandle, out uint lpMode);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetConsoleMode(nint hConsoleHandle, uint dwMode);
    }
}
