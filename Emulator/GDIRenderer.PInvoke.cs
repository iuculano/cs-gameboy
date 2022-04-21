using System;
using System.Runtime.InteropServices;

namespace axGB
{
    public partial class GDIRenderer
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern IntPtr GetDC(IntPtr hwnd);

        [DllImport("gdi32.dll")]
        private static extern IntPtr CreateCompatibleDC(IntPtr hdc);


        [DllImport("gdi32.dll")]
        private static extern IntPtr CreateDIBSection(
            IntPtr          hdc,
            ref BITMAPINFO  pbmi,
            uint            usage,
            out IntPtr      ppvBits,
            IntPtr          hSection,
            uint            offset
        );

        [DllImport("gdi32.dll")]
        private static extern bool BitBlt(
            IntPtr hdc,
            int    x,
            int    y,
            int    cx,
            int    cy,
            IntPtr hdcSrc,
            int    x1,
            int    y1,
            uint   rop
        );

        [DllImport("gdi32.dll")]
        private static extern bool ReleaseDC(IntPtr hwnd, IntPtr hdc);

        [DllImport("gdi32.dll")]
        private static extern bool DeleteDC(IntPtr hdc);

        [DllImport("gdi32.dll")]
        private static extern IntPtr SelectObject(IntPtr hdc, IntPtr h);

        [DllImport("gdi32.dll")]
        private static extern bool DeleteObject(IntPtr ho);

        [StructLayout(LayoutKind.Sequential)]
        private struct BITMAPINFO
        {
            public Int32 biSize;
            public Int32 biWidth;
            public Int32 biHeight;
            public Int16 biPlanes;
            public Int16 biBitCount;
            public Int32 biCompression;
            public Int32 biSizeImage;
            public Int32 biXPelsPerMeter;
            public Int32 biYPelsPerMeter;
            public Int32 biClrUsed;
            public Int32 biClrImportant;
        }
    }
}
