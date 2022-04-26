using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace axGB
{
    public partial class GDIRenderer : IDisposable
    {
        // These are all unmanaged
        private IntPtr hwnd;
        private IntPtr hdc;
        private IntPtr hdcBackbuffer;
        private IntPtr hdcBackbufferState;
        private IntPtr dib;
        private IntPtr data;
        private bool   isDisposed;
        public int     width  { get; init; }
        public int     height { get; init; }
        public GDIRenderer(int width, int height)
        {
            this.width  = width;
            this.height = height;

            var bi = new BITMAPINFO()
            {
                biSize     = Marshal.SizeOf(typeof(BITMAPINFO)),
                biWidth    = width,
                biHeight   = -height,  // Top down, otherwise it's created bottom up
                biPlanes   = 1,
                biBitCount = 32
            };

            hwnd               = GetForegroundWindow(); // Hack to grab handle to our own window
            hdc                = GetDC(hwnd);
            hdcBackbuffer      = CreateCompatibleDC(hdc);
            dib				   = CreateDIBSection(hdc, ref bi, 0, out data, dib, 0); // DIB_RGB_COLORS = 0
            hdcBackbufferState = SelectObject(hdcBackbuffer, dib);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetPixel(int x, int y, uint color)
        {
            // This can technically write out of bounds memory but the emulator itself
            // never will rendering a game.
            // if((x | y) >= 0 && x < width && y < height)
            unsafe
            {
                uint* ptr          = (uint*)data.ToPointer();
                ptr[y * width + x] = color;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Flip()
        {
            // Not really a flip, but...
            BitBlt(hdc, 128, 0, width, height, hdcBackbuffer, 0, 0, 0x00CC0020); // SRCCOPY = 0x00CC0020
            unsafe
            {
                // Clear the screen
                uint* ptr = (uint*)data.ToPointer();
                for (int y = 0; y <= height; y++)
                {
                    for (int x = 0; x <= width; x++)
                    {
                        ptr[y * width + x] = 0;
                    }
                }
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // I think this is correct???
                SelectObject(hdcBackbuffer, hdcBackbufferState); // Restore the original object back in...
                DeleteObject(dib);
                DeleteDC(hdcBackbuffer);
                ReleaseDC(hwnd, hdc);				

                isDisposed = true;
            }
        }

        ~GDIRenderer()
        {
            // Dispose(disposing: false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
