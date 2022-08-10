using MonoEye.Common;
using System.Diagnostics;
using System.Drawing;

namespace PakExtract
{
    class BlockRenderer
    {
        public VcnFile Vcn { get; set; }
        public VmpFile Vmp { get; set; }
        public PaletteFile Palette { get; set; }

        public BlockRenderer(VcnFile vcn, VmpFile vmp, PaletteFile pal)
        {
            Vcn = vcn;
            Vmp = vmp;
            Palette = pal;
        }

        public void DrawWall(Bitmap bmp, int wallType, int wallPosition, int offsetX, int offsetY)
        {
            int offset = WallRenderData.Data[wallPosition].BaseOffset;

            short visibleHeightInBlocks = WallRenderData.Data[wallPosition].VisibleHeightInBlocks;
            short visibleWidthInBlocks = WallRenderData.Data[wallPosition].VisibleWidthInBlocks;
            int flipX = WallRenderData.Data[wallPosition].FlipFlag;

            for (int y = 0; y < visibleHeightInBlocks; y++)
            {    
                for (int x = 0; x < visibleWidthInBlocks; x++)
                {
                    int blockIndex;
                    if (flipX == 0)
                        blockIndex = x + y * 22 + WallRenderData.Data[wallPosition].OffsetInViewPort;
                    else
                        blockIndex = WallRenderData.Data[wallPosition].OffsetInViewPort +
                                   visibleWidthInBlocks - 1 - x + y * 22;

                    int xpos = blockIndex % 22;
                    int ypos = blockIndex / 22;
                    int tile = Vmp.WallTiles[wallType, offset];

                    /* xor with wall flip-x to make block flip and wall flip cancel each other out. */
                    int blockFlip = (tile & 0x4000) ^ flipX;
                    blockIndex = tile & 0x3fff;

                    DrawBlock(bmp, xpos * 8, ypos * 8, blockIndex, blockFlip > 0, offsetX, offsetY);

                    offset++;
                }
                offset += WallRenderData.Data[wallPosition].SkipValue;
            }
        }

        private void DrawBlock(Bitmap bmp, int xpos, int ypos, int blockIndex, 
            bool isFlipped, int offsetX = 0, int offsetY = 0)
        {
            var blockData = Vcn.Blocks[blockIndex];

            for (int y = 0; y < 8; y++)
                for (int x = 0; x < 8; x++)
                {
                    var val = (isFlipped) ? blockData[7- x, y] : blockData[x, y];
                    if (val == 0)
                        continue;
                    var col = Palette.Colors[Vcn.WallLookup[val]];
                    bmp.SetPixel(xpos + x + offsetX, ypos + y + offsetY, col);
                }
        }

        public Bitmap DrawBackdrop()
        {
            Bitmap bmp = new Bitmap(176, 120);
            DrawBackdrop(bmp, 0, 0);
            return bmp;
        }
        public void DrawBackdrop(Bitmap bmp, int offsetX, int offsetY)
        {
            for (int y = 0; y < 15; y++)
                for (int x = 0; x < 22; x++)
                {
                    var tile = Vmp.BackgroundTiles[x, y];
                    bool isLimit = (tile & 0b10000000_00000000) > 0;
                    bool isFlipped = (tile & 0b01000000_00000000) > 0;

                    if (isLimit || isFlipped)
                        Debug.WriteLine(isLimit + " " + isFlipped);

                    var block = Vcn.Blocks[tile & 0x3fff];
                    for (int y2 = 0; y2 < 8; y2++)
                        for (int x2 = 0; x2 < 8; x2++)
                        {
                            var val = block[x2, y2];
                            var col = Palette.Colors[Vcn.BackdropLookup[val]];
                            bmp.SetPixel(x * 8 + x2 + offsetX, y * 8 + y2 + offsetY, col);
                        }
                }
        }
    }
}
