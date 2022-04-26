using System;
using axGB.System;
using System.Drawing;


namespace axGB.CPU
{
    public partial class GraphicsProcessor
    {
        public const int ScreenWidth  = 160;
        public const int ScreenHeight = 144;

        private enum Mode : byte
        {
            HBlank    = 0b_00000000,
            VBlank    = 0b_00000001,
            OAM       = 0b_00000010,
            Transfer  = 0b_00000011
        }

        public enum LCDControl : byte
        {
            LCDEnabled            = 0b_10000000,
            WindowArea            = 0b_01000000,
            WindowEnabled         = 0b_00100000,
            BackgroundDataArea    = 0b_00010000,
            BackgroundTileMapArea = 0b_00001000,
            ObjectSize            = 0b_00000100,
            ObjectEnabled         = 0b_00000010,
            ObjectPriorityEnabled = 0b_00000001,
        }

        private MemoryBus   memory;
        private int         cycles;
        private uint[]      pallete;

        private GDIRenderer gdi;



        public void InitRenderer()
        {
            gdi = new GDIRenderer(ScreenWidth, ScreenHeight);
        }

        public void DrawTileScanline(ReadOnlySpan<byte> data, int tileCoordinateX, int tileScanline, int scanline)
        {
            // https://www.huderlem.com/demos/gameboy2bpp.html

            // 2 bytes make up a row, y is multiplied to compensate for this
            var one = data[(tileScanline * 2)];
            var two = data[(tileScanline * 2) + 1];

            // Walk through the bits of the 2 bytes
            for (int _x = 0; _x < 8; _x++)
            {
                // Mask to grab current bit
                var bit   = 0b_10000000 >> _x;

                // Check if said bit is set on the low byte, then high byte
                var low   = ((one & bit) > 0) ? 0b_00000001 : 0;
                var high  = ((two & bit) > 0) ? 0b_00000010 : 0;
                var color = (high | low);

                // x is the location we're at in the tile grid, need to map to where
                // in the framebuffer we're writing
                var ix    = (tileCoordinateX * 8) + _x;
                gdi.SetPixel(ix, scanline, pallete[color]);
            }
        }

        public void WalkTileMapRow(int scanline)
        {
            // https://gbdev.io/pandocs/LCDC.html
            var tileData = ((memory.LCDC & (byte)LCDControl.BackgroundDataArea)    == 0) ? 0x8800 : 0x8000;
            var tileMap  = ((memory.LCDC & (byte)LCDControl.BackgroundTileMapArea) == 0) ? 0x9800 : 0x9C00;
            var window   =  (memory.LCDC & (byte)LCDControl.WindowEnabled)         == 0;

            // Figure out what row needs to be drawn, and what line from within the tiles
            var scanlineToTileIndex = (scanline * 18) / 144;
            var tileScanline        = scanline % 8;

            // Get tile indicies and pointer to their data for a given scanline
            for (int x = 0; x < 20; x++)
            {
                var viewY = scanlineToTileIndex + memory.SCY;
                var viewX = x + memory.SCX;

                // The background tile map is 32x32
                var index = ((viewY * 32) + viewX);
                var id    = memory.Memory[tileMap + index];

                // Each tile is 16 bytes, so finding the right tile and 
                // multiplying by 16 will point at the right spot in memory
                var ptr   = tileData + (id * 16);
                var span  = new ReadOnlySpan<byte>(memory.Memory, ptr, 16);

                DrawTileScanline(span, x, tileScanline, scanline);
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
            pallete = new uint[4]
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
            var mode = (Mode)(memory.STAT & 0b_00000011);
            switch (mode)
            {
                case Mode.HBlank:
                    if (this.cycles >= 204)
                    {
                        // We've written an entire scanline
                        memory.LY++;

                        // 144 - 153 are v-blank
                        if (memory.LY >= 144)
                        {
                            Flip();

                            if ((memory.IE & 0b_00000001) > 0)
                            {
                                memory.IF |= 0b_00000001;
                            }

                            memory.STAT = (int)Mode.VBlank;
                        }

                        else
                        {
                            // http://imrannazar.com/GameBoy-Emulation-in-JavaScript:-GPU-Timings ???
                            memory.STAT = (int)Mode.OAM; // OAM
                        }

                        this.cycles -= 204;
                    }

                    break;

                case Mode.VBlank:
                    if (this.cycles >= 456)
                    {
                        memory.LY++;

                        if (memory.LY >= 154)
                        {
                            // I think blow away LY and jump to OAM?
                            memory.LY   = 0;
                            memory.STAT = (int)Mode.OAM;
                        }

                        this.cycles -= 456;
                    }

                    break;

                case Mode.OAM:
                    if (this.cycles >= 80)
                    {
                        memory.STAT  = (int)Mode.Transfer;
                        this.cycles -= 80;
                    }

                    break;

                case Mode.Transfer:
                    if (this.cycles >= 172)
                    {
                        WalkTileMapRow(memory.LY);

                        memory.STAT  = (int)Mode.HBlank;
                        this.cycles -= 172;
                    }

                    break;
            }
        }
    }
}
