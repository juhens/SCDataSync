using System.Runtime.InteropServices;
using System.Text;

namespace SCDataSync.Memory.Native
{
    public static class Psapi
    {
        [DllImport("psapi.dll")]
        public static extern bool EnumProcessModulesEx(nint hProcess, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U4)][In][Out] nint[] lphModule, int cb, [MarshalAs(UnmanagedType.U4)] out int lpcbNeeded, uint dwFilterFlag);

        [DllImport("psapi.dll")]
        public static extern uint GetModuleFileNameEx(nint hProcess, nint hModule, [Out] StringBuilder lpBaseName, [In][MarshalAs(UnmanagedType.U4)] uint nSize);

        [DllImport("psapi.dll", SetLastError = true)]
        public static extern bool GetModuleInformation(nint hProcess, nint hModule, out ModuleInformation lpmodinfo, uint cb);
    }
}
