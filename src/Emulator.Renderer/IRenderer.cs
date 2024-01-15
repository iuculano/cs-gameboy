using System;
using Silk.NET.Windowing;

namespace Emulator.Renderer;

public interface IRenderer
{
    /// <summary>
    /// Creates a renderer, using the given window.
    /// </summary>
    /// <param name="window"></param>
    void Create(IWindow window);

    /// <summary>
    /// Destroys the renderer, cleaning up all resources.
    /// </summary>
    void Destroy();

    /// <summary>
    /// Renders a frame.
    /// </summary>
    /// <param name="data">32-bit pixel data.</param>
    void Render(ReadOnlySpan<uint> data);
}
