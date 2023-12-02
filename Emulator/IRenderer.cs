using Silk.NET.Windowing;
using System;

namespace axGB.Emulator
{
    public interface IRenderer
    {
        void Initialize(IWindow windowHandle);
        void Render    (ReadOnlySpan<uint> data);
    }
}
