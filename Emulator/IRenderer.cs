using System;
using Silk.NET.Windowing;

namespace axGB
{
    public interface IRenderer
    {
        void Initialize(IWindow windowHandle);
        void Render    (ReadOnlySpan<uint> data);
    }
}
