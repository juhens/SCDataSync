using System.Runtime.InteropServices;

namespace SCDataSync.Memory.Native
{
    public static class User32
    {
        [DllImport("user32.dll")]
        public static extern int DeleteMenu(nint hMenu, int nPosition, int wFlags);

        [DllImport("user32.dll")]
        public static extern nint GetSystemMenu(nint hWnd, bool bRevert);
    }
}
