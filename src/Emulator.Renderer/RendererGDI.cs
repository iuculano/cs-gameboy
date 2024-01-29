/*using Emulator.Renderer;
using Silk.NET.Windowing;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Enulator.Renderer;

public partial class RendererGDI : IRenderer, IDisposable
{
    // These are all unmanaged
    private IntPtr hwnd;
    private IntPtr hdc;
    private IntPtr hdcBackbuffer;
    private IntPtr hdcBackbufferState;
    private IntPtr dib;
    private IntPtr data;
    private bool   isDisposed;

    private int width ;
    private int height;

    public void Create(IWindow window)
    {
        width  = window.Size.X;
        height = window.Size.Y;

        var bi = new BITMAPINFO()
        {
            biSize     = Marshal.SizeOf(typeof(BITMAPINFO)),
            biWidth    = width,
            biHeight   = -height, // Top down, otherwise it's created bottom up
            biPlanes   = 1,
            biBitCount = 32
        };

        hwnd               = window.Handle;
        hdc                = GetDC(hwnd);
        hdcBackbuffer      = CreateCompatibleDC(hdc);
        dib				   = CreateDIBSection(hdc, ref bi, 0, out data, dib, 0); // DIB_RGB_COLORS = 0
        hdcBackbufferState = SelectObject(hdcBackbuffer, dib);
    }

    public RendererGDI(IWindow window)
    {
        Create(window);
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
    public void Render(ReadOnlySpan<uint> data)
    {
        // Not really a flip, but...
        BitBlt(hdc, 0, 0, width, height, hdcBackbuffer, 0, 0, 0x00CC0020); // SRCCOPY = 0x00CC0020
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

    ~RendererGDI()
    {
        // Dispose(disposing: false);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
*/