using axGB.CPU;
using axGB.System;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.Windowing;

namespace axGB
{
    class Program
    {
        private static IWindow   window;
        private static IRenderer renderer;
        private static double    accumulator;

        private static MemoryBus         memory;
        private static Processor         processor;
        private static GraphicsProcessor graphics;
        private static Timer             timer;
       

        private static void OnLoad()
        {
            renderer = new RendererGL(window);

            memory    = new MemoryBus();
            processor = new Processor(memory);
            timer     = new Timer(memory);
            graphics  = new GraphicsProcessor(memory);

            var cartridge = Cartridge.Load(@"tetris.gb", memory);
            memory.Connect(cartridge);
        }

        private static void OnUpdate(double deltaTime)
        {
            accumulator += deltaTime;

            var cycles          = 0;
            var cyclesThisFrame = 0;

            while (cyclesThisFrame < Processor.CyclesPerFrame)
            {
                cycles = processor.Update();
                timer.Update(cycles);
                graphics.Update(cycles);
                cyclesThisFrame += cycles;

                if (graphics.IsReadyToRender)
                {
                    renderer.Render(graphics.Backbuffer);
                }
            }
        }

        private static void KeyDown(IKeyboard arg1, Key arg2, int arg3)
        {
            if (arg2 == Key.Escape)
            {
                window.Close();
            }
        }

        static void Main(string[] args)
        {
            var options   = WindowOptions.Default;
            options.Size  = new Vector2D<int>(800, 720);
            options.VSync = false;

            // Window needs to be created before we can set up the renderer
            window         = Window.Create(options);
            window.Load   += OnLoad;
            window.Update += OnUpdate;

            window.Run();
            window.Dispose();
        }
    }
}
