using System;
using axGB.System;
using System.Drawing;


namespace axGB.CPU
{
    public partial class GraphicsProcessor
    {
        public const int ScreenWidth  = 160;
        public const int ScreenHeight = 144;


        private MemoryBus memory;
        private int       cycles;
        private uint[]    pallete;

        private GDIRenderer gdi;


        public void InitRenderer()
        {
            gdi = new GDIRenderer(ScreenWidth, ScreenHeight);
        }

        public void DrawTileScanline(ReadOnlySpan<byte> data, int x, int y)
        {
            // https://www.huderlem.com/demos/gameboy2bpp.html

            // 2 bytes make up a row, y is multiplied to compensate for this
            var one = data[y * 2];
            var two = data[y * 2 + 1];

            // Walk through the bits of the 2 bytes
            for (int _x = 0; _x < 8; _x++)
            {
                // Mask to grab selected bit
                var bit   = 0b_1000_0000 >> _x;

                // Check if said bit is set on the low byte, then high byte
                var low   = ((one & bit) > 0) ? 0b_0000_0001 : 0;
                var high  = ((two & bit) > 0) ? 0b_0000_0010 : 0;
                var color = (high | low);

                // Need to multiply by the width of a tile, x represents the
                // location in tile grid
                var ix   = (x * 8) + _x;
                gdi.SetPixel(ix, memory.LY, pallete[color]);
            }
        }

        public void WalkTileMapScanline(int scanline)
        {
            // https://gbdev.io/pandocs/LCDC.html
            var tileData = ((memory.LCDC & 0b_0001_0000) == 0) ? 0x8800 : 0x8000;
            var tileMap  = ((memory.LCDC & 0b_0000_1000) == 0) ? 0x9800 : 0x9C00;
            var window   =  (memory.LCDC & 0b_0001_0000) == 0;

            // Walk horizontally across the tiles on a given scanline
            var scanlineToTileIndex = (int)MathF.Floor((float)(scanline * 18) / 144);
            var tileScanline        = scanline % 8;

            for (int x = 0; x < 20; x++)
            {
                var viewY = scanlineToTileIndex + memory.SCY;
                var viewX = x + memory.SCX;

                // The background tile map is 32x32, we can simply grab the index
                // with (y * width) + x as it's just a 2D array
                var index = ((viewY * 32) + viewX);
                var id    = memory.Memory[tileMap + index];

                // Each tile is 16 bytes, so finding the right tile and 
                // multiplying by 16 will point at the right spot in memory
                var ptr   = tileData + (id * 16);
                var span  = new ReadOnlySpan<byte>(memory.Memory, ptr, 16);

                DrawTileScanline(span, x, tileScanline);
            }
        }

        public void Flip()
        {
            gdi.Flip();
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

        ~GraphicsProcessor()
        {
            gdi.Dispose();
        }

        [Flags]
        private enum GPUMode
        {
            HBlank   = 0b_00000000,
            VBlank   = 0b_00000001,
            OAM      = 0b_00000010,
            Transfer = 0b_00000011
        }

        public void Update(int cycles)
        {
            // https://www.reddit.com/r/Gameboy/comments/a1c8h0/what_happens_when_a_gameboy_screen_is_disabled/
            /*var what = memory.LCDC & 0b_10000000;
            if (what == 0)
            {
                //this.cycles = 0;
                //memory.LY   = 0;
                //memory.STAT = 0b_00000000;

                return this.cycles;
            }*/

            this.cycles += cycles;

            // https://gbdev.io/pandocs/STAT.html
            var mode = (GPUMode)(memory.STAT & 0b_00000011);
            switch (mode)
            {
                case GPUMode.HBlank:
                    if (this.cycles >= 204)
                    {
                        // We've written an entire scanline
                        memory.LY++;

                        // 144 - 153 are v-blank
                        if (memory.LY >= 144)
                        {
                            Flip();

                            // Should fire an interupt here?
                            if ((memory.IE & 0b_00000001) > 0)
                            {
                                memory.IF |= 0b_00000001;
                            }

                            memory.STAT = (int)GPUMode.VBlank;
                        }

                        else
                        {
                            // http://imrannazar.com/GameBoy-Emulation-in-JavaScript:-GPU-Timings ???
                            memory.STAT = (int)GPUMode.OAM; // OAM
                        }

                        this.cycles -= 204;
                    }

                    break;

                case GPUMode.VBlank:
                    if (this.cycles >= 456)
                    {
                        memory.LY++;

                        if (memory.LY >= 154)
                        {
                            // I think blow away LY and jump to OAM?
                            memory.LY   = 0;
                            memory.STAT = (int)GPUMode.OAM;
                        }

                        this.cycles -= 456;
                    }

                    break;

                case GPUMode.OAM:
                    if (this.cycles >= 80)
                    {
                        memory.STAT  = (int)GPUMode.Transfer;
                        this.cycles -= 80;
                    }

                    break;

                case GPUMode.Transfer:
                    if (this.cycles >= 172)
                    {
                        WalkTileMapScanline(memory.LY);

                        memory.STAT  = (int)GPUMode.HBlank;
                        this.cycles -= 172;
                    }

                    break;
            }
        }
    }
}
