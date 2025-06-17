namespace DofusGroupFinder.Client.Utils
{
    internal static class NativeMethods
    {
        internal static readonly nint HWND_TOPMOST = new nint(-1);
        internal const uint SWP_NOMOVE = 0x0002;
        internal const uint SWP_NOSIZE = 0x0001;

        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        internal static extern bool SetWindowPos(nint hWnd, nint hWndInsertAfter,
            int X, int Y, int cx, int cy, uint uFlags);
    }
}