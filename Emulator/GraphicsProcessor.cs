using System;
using axGB.System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;


namespace axGB.CPU
{
    public partial class GraphicsProcessor
    {
        private MemoryBus memory;
        private int       cycles;
        
        private IntPtr    hwnd;
        private Graphics  graphics;
        private Bitmap    backbuffer;
        private Bitmap    vram;
        private uint[]    pallete;


        // This is a hack to grab the HWND of the console
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        public void InitRenderer()
        {
            hwnd       = GetForegroundWindow();
            graphics   = Graphics.FromHwnd(hwnd);
            backbuffer = new Bitmap(160, 144, PixelFormat.Format32bppRgb);
            vram       = new Bitmap(160, 144, PixelFormat.Format32bppRgb);
        }

        public void DrawVram(ReadOnlySpan<byte> data, int x, int y)
        {

        }

        public void DrawTile(Bitmap destination, ReadOnlySpan<byte> data, int x, int y)
        {
            var rect   = new Rectangle(0, 0, destination.Width, destination.Height);
            var buffer = destination.LockBits(rect, ImageLockMode.ReadWrite, destination.PixelFormat);
            var width  = destination.Width;

            // https://www.huderlem.com/demos/gameboy2bpp.html was super useful
            // Row
            for (int _y = 0; _y < 16; _y += 2)
            {
                var one = data[_y];
                var two = data[_y + 1];

                for (int _x = 0; _x < 8; _x++)
                {
                    // Grab the given bit
                    var bit   = 0x80 >> _x;
                    var high  = ((two & bit) > 0) ? 2 : 0;
                    var low   = ((one & bit) > 0) ? 1 : 0;
                    var color = (high | low);

                    var ix    = (x * 8) + _x;
                    var iy    = (y * 8) + _y / 2;
                    var index = (iy * width) + ix;

                    unsafe
                    {
                        uint* ptr  = (uint*)buffer.Scan0;
                        ptr[index] = pallete[color];
                    }
                }
            }

            destination.UnlockBits(buffer);
        }

        public void WalkTileMap()
        {
            // https://gbdev.io/pandocs/LCDC.html
            var tileData = ((memory.LCDC & 0b_0001_0000) == 0) ? 0x8800 : 0x8000;
            var tileMap  = ((memory.LCDC & 0b_0000_1000) == 0) ? 0x9800 : 0x9C00;
            var window   =  (memory.LCDC & 0b_0001_0000) == 0;


            // Iterate over bg tiles
            for (int y = 0; y < 18; y++)
            {
                for (int x = 0; x < 20; x++)
                {
                    // Each tile is 16 bytes, so finding the right tile and 
                    // multiplying by 16 will point at the right spot in memory
                    var viewY = y + (window ? memory.WY : memory.SCY);
                    var viewX = x + (window ? memory.WX : memory.SCX);

                    var index = ((viewY * 32) + viewX);
                    var id    = memory.Memory[tileMap + index];
                    var ptr   = tileData + (id * 16);

                    
                    var span  = new ReadOnlySpan<byte>(memory.Memory, ptr, 16);

                    DrawTile(backbuffer, span, x, y);
                }
            }

            #if DEBUG
            // This will draw the contents of VRAM to screen
            
            for (int y = 0; y < 18; y++)
            {
                for (int x = 0; x < 20; x++)
                {
                    // Each tile is 16 bytes, so finding the right tile and 
                    // multiplying by 16 will point at the right spot in memory
                    var index = ((y * 16) + x) * 16;
                    var ptr   = tileData + index;            
                    var span  = new ReadOnlySpan<byte>(memory.Memory, ptr, 16);

                    DrawTile(vram, span, x, y);
                }
            }
            #endif
        }

        public void Flip()
        {
            WalkTileMap();
            graphics.DrawImage(backbuffer, 128, 128);
            graphics.DrawImage(vram, 128 + backbuffer.Width + 1, 128);
        }


        public GraphicsProcessor(MemoryBus memory)
        {
            InitRenderer();
            this.memory = memory;
            pallete     = new uint[4]
            {
                // Thank you Adobe color wheel - yes, it's hideous
                (uint)Color.FromArgb(0xF5, 0x9A, 0x9C).ToArgb(),
                (uint)Color.FromArgb(0x89, 0x5A, 0x94).ToArgb(),
                (uint)Color.FromArgb(0xA7, 0xA0, 0xEB).ToArgb(),
                (uint)Color.FromArgb(0x32, 0x43, 0x31).ToArgb()
            };
        }


        public int Update(int cycles)
        {
            // https://www.reddit.com/r/Gameboy/comments/a1c8h0/what_happens_when_a_gameboy_screen_is_disabled/
            var what = memory.LCDC & 0b_10000000;
            if (what == 0)
            {
                this.cycles = 0;
                memory.LY   = 0;
                memory.STAT = 0b_00000000;
            }

            // delta     = cycles - this.cycles;
            this.cycles += cycles;

            // https://gbdev.io/pandocs/STAT.html
            // I quite honestly still have no clue how the timing here works
            // This is probably wrong, but seemingly good enough to render the
            // bootstrap rom
            switch (memory.STAT & 0b_00000011)
            {
                case 0b_00000000: // H-Blank
                    if (this.cycles >= 204) // This seems wrong, is this variable?
                    {
                        // We've written an entire scanline
                        memory.LY++;

                        // 144 - 153 are v-blank
                        if (memory.LY >= 144)
                        {
                            // Should fire an interupt here?
                            memory.STAT = 0b_00000001;
                        }

                        else
                        {
                            // http://imrannazar.com/GameBoy-Emulation-in-JavaScript:-GPU-Timings ???
                            memory.STAT = 0b_00000010; // OAM
                        }

                        this.cycles -= 204;
                    }
                    
                    break;

                case 0b_00000001: // V-Blank
                    if (this.cycles >= 456) // is this right??
                    {
                        Flip();
                        memory.LY++;

                        if (memory.LY >= 154)
                        {
                            // I think blow away LY and jump to OAM?
                            memory.LY   = 0;
                            memory.STAT = 0b_00000010; // OAM
                        }

                        this.cycles -= 456;
                    }
                    
                    break;

                case 0b_00000010: // OAM
                    if (this.cycles >= 80)
                    {
                        memory.STAT = 0b_00000011;
                        this.cycles -= 80;
                    }
                    
                    break;

                case 0b_00000011: // Transfer
                    if (this.cycles >= 172)
                    {
                        memory.STAT = 0b_00000000;
                        this.cycles -= 172;
                    }

                    break;

            }


            return this.cycles;
        }
    }
}
