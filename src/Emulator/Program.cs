using Enulator.Core.Cartridge;
using Enulator.Core.CPU;
using Enulator.Core.PPU;

using Emulator.Renderer;

using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.Windowing;
using Emulator.Core.Bus;
using Emulator.Core.Input;
using Enulator.Core.Bus;


namespace Emulator;

class Program
{
    private static IWindow   window;
    private static IRenderer renderer;
    private static double    accumulator;

    private static MemoryBus         memory;
    private static InterruptHandler  interruptHandler;
    private static Processor         processor;
    private static GraphicsProcessor graphics;
    private static Timer             timer;
    private static Joypad            joypad;
   

    private static void OnLoad()
    {
        var input                   = window.CreateInput();
        input.Keyboards[0].KeyDown += OnKeyDown;
        input.Keyboards[0].KeyUp   += OnKeyUp;

        renderer = new RendererGL(window);

        memory           = new MemoryBus();
        interruptHandler = new InterruptHandler(memory);

        
        processor = new Processor(memory, interruptHandler);
        graphics  = new GraphicsProcessor(memory, interruptHandler);
        timer     = new Timer(memory, interruptHandler);
        joypad    = new Joypad(memory, interruptHandler);            

        var cartridge = Cartridge.Load(@"03-op sp,hl.gb", memory);
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
            joypad.Update();
            cyclesThisFrame += cycles;

            if (graphics.IsReadyToRender)
            {
                renderer.Render(graphics.Backbuffer);
            }
        }
    }

    private static JoypadButton ConvertKeyCodes(Key key)
    {
        switch (key)
        {
            case Key.W:     return JoypadButton.Up;
            case Key.A:     return JoypadButton.Left;
            case Key.S:     return JoypadButton.Down;
            case Key.D:     return JoypadButton.Right;
            case Key.Left:  return JoypadButton.Select;
            case Key.Up:    return JoypadButton.Start;
            case Key.Right: return JoypadButton.A;
            case Key.Down:  return JoypadButton.B;
            default:        return JoypadButton.Unknown;
        }
    }

    private static void OnKeyDown(IKeyboard keyboard, Key key, int state)
    {
        var button = ConvertKeyCodes(key);
        joypad.SetKeyDown(button);
    }

    private static void OnKeyUp(IKeyboard keyboard, Key key, int state)
    {
        var button = ConvertKeyCodes(key);
        joypad.SetKeyUp(button);
    }

    static void Main(string[] args)
    {
        var options   = WindowOptions.Default;
        options.Size  = new Vector2D<int>(800, 720);
        options.VSync = true;

        // Window needs to be created before we can set up the renderer
        window         = Window.Create(options);
        window.Load   += OnLoad;
        window.Update += OnUpdate;

        window.Run();
        window.Dispose();
    }
}
