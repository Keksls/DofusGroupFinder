namespace DofusGroupFinder.Client
{
    internal static class NativeMethods
    {
        internal static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        internal const UInt32 SWP_NOMOVE = 0x0002;
        internal const UInt32 SWP_NOSIZE = 0x0001;

        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        internal static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter,
            int X, int Y, int cx, int cy, uint uFlags);
    }
}