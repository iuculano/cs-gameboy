using System;
using System.Drawing;
using System.Runtime.CompilerServices;
using Emulator.Core.Bus;
using Enulator.Core.Bus;

namespace Enulator.Core.PPU
{
    public partial class GraphicsProcessor
    {
        public const int ScreenWidth  = 160;
        public const int ScreenHeight = 144;

        private enum LCDMode : byte
        {
            HBlank   = 0b_00000000,
            VBlank   = 0b_00000001,
            OAM      = 0b_00000010,
            Transfer = 0b_00000011
        }

        public enum LCDControl : byte
        {
            LCDEnabled             = 0b_10000000,
            WindowArea             = 0b_01000000,
            WindowEnabled          = 0b_00100000,
            BackgroundDataArea     = 0b_00010000,
            BackgroundTileMapArea  = 0b_00001000,
            ObjectSize             = 0b_00000100,
            ObjectEnabled          = 0b_00000010,
            BackgroundWindowEnable = 0b_00000001,
        }

        private readonly MemoryBus        memory;
        private readonly InterruptHandler interruptHandler;

        private readonly uint[] palette;
        private int cycles;

        // Translated framebuffer
        public uint[] Backbuffer    { get; private set; } = new uint[ScreenWidth * ScreenHeight];
        public uint[] Background    { get; private set; } = new uint[256 * 256];
        public bool IsReadyToRender { get; private set; }

        public void DrawSpriteScanLine()
        {
            throw new NotSupportedException();
        }

        public void DrawTileScanline(ReadOnlySpan<byte> data, int tileCoordinateX, int tileScanline, int scanline)
        {
            // https://www.huderlem.com/demos/gameboy2bpp.html

            // 2 bytes make up an entire row - 16 bytes for the entire tile
            // y is multiplied to compensate for this, for example:
            // 0 * 2 = 0 - byte 0 and 1 comprise a row
            // 1 * 2 = 2 - byte 2 and 3 comprise a row
            // 2 * 2 = 4 - byte 4 and 5 comprise a row...
            var rowIndex = tileScanline * 2; // This saves an instruction somehow
            var one      = data[rowIndex];
            var two      = data[rowIndex + 1];

            // Walk through the bits of the 2 bytes, most significant to least
            for (var x = 0; x < 8; x++)
            {
                // Mask to grab just the current bit handling
                var bit = 0b_10000000 >> x;

                // 2 bits per pixel, 0b_000000HL
                // Shift into position depending on which bit we're on
                var low   = (one & bit) > 0 ? 0b_00000001 : 0; // 0b_0000000L
                var high  = (two & bit) > 0 ? 0b_00000010 : 0; // 0b_000000H0
                var color = high | low;                        // 0b_000000HL

                // Need to map to where in the framebuffer we're writing
                // Each tile is 8 pixels - multiply by 8 to go from tile-space
                // to pixel-space, plus x for the pixel in the row we're on
                var pixelIndex = tileCoordinateX * 8 + x;
                Backbuffer[scanline * ScreenWidth + pixelIndex] = palette[color];
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte DecodeTileData(ReadOnlySpan<byte> data, int column, int scanline)
        {
            // https://www.huderlem.com/demos/gameboy2bpp.html

            // 2 bytes make up an entire row - 16 bytes for the entire tile
            // Grab the entire row we're on first
            // y is multiplied to compensate for this, for example:
            // 0 * 2 = 0 - byte 0 and 1 comprise a row
            // 1 * 2 = 2 - byte 2 and 3 comprise a row
            // 2 * 2 = 4 - byte 4 and 5 comprise a row...
            var rowIndex = scanline * 2;       // This saves an instruction somehow
            var one      = data[rowIndex];     // byte 1
            var two      = data[rowIndex + 1]; // byte 2

            // Mask to grab just the current bit handling
            var bit = 0b_10000000 >> column;

            // 2 bits per pixel, 0b_000000HL
            // Shift into position depending on which bit we're on
            var low  = (one & bit) > 0 ? 0b_00000001 : 0; // 0b_0000000L
            var high = (two & bit) > 0 ? 0b_00000010 : 0; // 0b_000000H0
            return (byte)(high | low);                    // 0b_000000HL
        }

        public void WalkScanline(int scanline)
        {
            // https://gbdev.io/pandocs/LCDC.html
            var lcdc = (LCDControl)memory.LCDC;
            if (lcdc.HasFlag(LCDControl.BackgroundWindowEnable))
            {
                // Memory locations can differ depending how the register is set
                // Window is not implemented
                var tileDataOffset = lcdc.HasFlag(LCDControl.BackgroundDataArea)    ? 0x0000 : 0x0800;
                var tileMapOffset  = lcdc.HasFlag(LCDControl.BackgroundTileMapArea) ? 0x1C00 : 0x1800;
                var window         = lcdc.HasFlag(LCDControl.WindowEnabled)         ? 0x0000 : 0x0000;

                for (var x = 0; x < ScreenWidth; x++)
                {
                    // Figure out which pixel we should be drawing out of the 256x256 background
                    // As we draw the entire scanline at once, we just need to add the scroll registers
                    // to get the correct offset
                    var backgroundX = (byte)(x + memory.SCX);
                    var backgroundY = (byte)(scanline + memory.SCY);

                    // Since tiles are 8x8, we just divide the coordinates in background/pixel space
                    // by 8 to find the correct tile coordinate in the 32x32 tile grid
                    var tileX = backgroundX / 8;
                    var tileY = backgroundY / 8;

                    // Now we need to find the appropriate entry in the tile map (which is 32x32)
                    // Each entry is 1 byte - pretend like we're indexing into a 2D array: (y * width) + x
                    var offset = tileY * 32 + tileX;
                    var index  = tileMapOffset + offset;

                    // https://gbdev.io/pandocs/Tile_Data.html#vram-tile-data
                    byte id = 0;
                    if (tileDataOffset == 0)
                    {
                        id = memory.VRAM[index];
                    }

                    // Need to use signed addressing if LCDControl.BackgroundDataArea is set
                    else
                    {
                        // Not sure if this is actually right???
                        // Something seems to be off, 0x9000 tiles don't seem to render right
                        id = (byte)(memory.VRAM[index] + 128);
                    }

                    // Get the memory location of the appropriate tile's actual graphic data
                    // Each tile is 16 bytes, so the offset is just id * 16
                    var ptr  = tileDataOffset + id * 16;
                    var data = new ReadOnlySpan<byte>(memory.VRAM, ptr, 16);

                    // Figure out where we are in the tile - each tile is 8x8 pixels
                    // Just wrap with modulo:
                    // 0 - 7
                    // 8 - 15
                    // For example, an x of 8 is pixel 0 in the second tile
                    var tileDataColumn   = backgroundX % 8;
                    var tileDataScanline = backgroundY % 8;

                    // Get the color for the pixel in the tile, it's just an index into the palette
                    var color = DecodeTileData(data, tileDataColumn, tileDataScanline);
                    Backbuffer[scanline * ScreenWidth + x] = palette[color];

                }
            }

            else
            {
                Console.WriteLine("Background disabled - sprite drawing stubbed.");
            }
        }

        public GraphicsProcessor(MemoryBus memory, InterruptHandler interruptHandler)
        {
            this.memory           = memory;
            this.interruptHandler = interruptHandler;

            palette = new uint[4]
            {
                (uint)Color.FromArgb(0x9B, 0xBC, 0x0F).ToArgb(),
                (uint)Color.FromArgb(0x8B, 0xAC, 0x0F).ToArgb(),
                (uint)Color.FromArgb(0x30, 0x62, 0x30).ToArgb(),
                (uint)Color.FromArgb(0x0F, 0x38, 0x0F).ToArgb()
            };

            // Match BGB's behavior here
            memory.LCDC = 0;
            memory.STAT = 0b_10000100; // 7 is always set it seems, start in Mode 3
        }

        ~GraphicsProcessor()
        {

        }

        public void Update(int cycles)
        {
            IsReadyToRender  = false;
            this.cycles     += cycles;

            bool lcdEnabled = (memory.LCDC & 0b_10000000) > 0;
            if (lcdEnabled)
            {
                // https://gbdev.io/pandocs/STAT.html
                var mode = (LCDMode)(memory.STAT & 0b_00000011);
                switch (mode)
                {
                    case LCDMode.HBlank:
                        if (this.cycles >= 204)
                        {
                            // We've written an entire scanline
                            memory.LY++;

                            // 144 - 153 are v-blank
                            if (memory.LY >= 144)
                            {
                                // Don't directly render here, rather signal that we're ready to render
                                IsReadyToRender = true;

                                memory.STAT = (int)LCDMode.VBlank;
                                interruptHandler.Request(InterruptType.VBlank);
                            }

                            else
                            {
                                // http://imrannazar.com/GameBoy-Emulation-in-JavaScript:-GPU-Timings ???
                                memory.STAT = (int)LCDMode.OAM; // OAM

                                // Depending on STAT, request and LCD interrupt
                                if ((memory.STAT & 0b_00001000) == 0b_00001000)
                                {
                                    interruptHandler.Request(InterruptType.LCD);
                                }
                            }

                            this.cycles -= 204;
                        }

                        break;

                    case LCDMode.VBlank:
                        if (this.cycles >= 456)
                        {
                            memory.LY++;

                            if (memory.LY >= 154)
                            {
                                // I think blow away LY and jump to OAM?
                                memory.LY   = 0;
                                memory.STAT = (int)LCDMode.OAM;

                                if ((memory.STAT & 0b_00010000) == 0b_00010000)
                                {
                                    interruptHandler.Request(InterruptType.LCD);
                                }
                            }

                            this.cycles -= 456;
                        }

                        break;

                    case LCDMode.OAM:
                        if (this.cycles >= 80)
                        {
                            memory.STAT  = (int)LCDMode.Transfer;
                            this.cycles -= 80;

                            if ((memory.STAT & 0b_00100000) == 0b_00100000)
                            {
                                interruptHandler.Request(InterruptType.LCD);
                            }
                        }

                        break;

                    case LCDMode.Transfer:
                        if (this.cycles >= 172)
                        {
                            WalkScanline(memory.LY);

                            memory.STAT  = (int)LCDMode.HBlank;
                            this.cycles -= 172;
                        }

                        break;
                }

                // I think this is OK to check here because this will update every
                // CPU step?
                // https://forums.nesdev.org/viewtopic.php?t=16434
                if (memory.LYC == memory.LY)
                {
                    // https://gbdev.io/pandocs/STAT.html#ff45--lyc-ly-compare
                    memory.STAT |= 0b_00000100;
                    interruptHandler.Request(InterruptType.LCD);
                }
            }

            else
            {
                // https://www.reddit.com/r/Gameboy/comments/a1c8h0/what_happens_when_a_gameboy_screen_is_disabled/
                memory.LY   = 0;
                memory.STAT = (byte)LCDMode.HBlank;
                this.cycles = 0;
            }
        }
    }
}
