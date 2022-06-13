using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace MonoEye.Common
{
    public class BlockRenderer
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

        public void DrawWall(Texture2D texture, int wallType, WallRenderData data, int offsetX, int offsetY)
        {
            int offset = data.BaseOffset;
            short visibleHeightInBlocks = data.VisibleHeightInBlocks;
            short visibleWidthInBlocks = data.VisibleWidthInBlocks;
            int flipX = data.FlipFlag;

            for (int y = 0; y < visibleHeightInBlocks; y++)
            {
                for (int x = 0; x < visibleWidthInBlocks; x++)
                {
                    int blockIndex;
                    if (flipX == 0)
                        blockIndex = x + y * 22 + data.OffsetInViewPort;
                    else
                        blockIndex = data.OffsetInViewPort +
                                   visibleWidthInBlocks - 1 - x + y * 22;

                    int xpos = blockIndex % 22;
                    int ypos = blockIndex / 22;
                    int tile = Vmp.WallTiles[wallType, offset];

                    /* xor with wall flip-x to make block flip and wall flip cancel each other out. */
                    int blockFlip = tile & 0x4000 ^ flipX;
                    blockIndex = tile & 0x3fff;

                    DrawBlock(texture, xpos * 8, ypos * 8, blockIndex, blockFlip > 0, offsetX, offsetY);

                    offset++;
                }
                offset += data.SkipValue;
            }
        }

        private void DrawBlock(Texture2D texture, int xpos, int ypos, int blockIndex,
            bool isFlipped, int offsetX = 0, int offsetY = 0)
        {
            var blockData = Vcn.Blocks[blockIndex];
            var colors = new Color[texture.Width * texture.Height];
            texture.GetData(colors);

            for (int y = 0; y < 8; y++)
                for (int x = 0; x < 8; x++)
                {
                    var val = isFlipped ? blockData[7 - x, y] : blockData[x, y];
                    if (val == 0)
                        continue;
                    var col = Palette.Colors[Vcn.WallLookup[val]];
                    colors[xpos + x + offsetX + (ypos + y + offsetY) * texture.Width] = col;
                }

            texture.SetData(colors);
        }

        public Texture2D DrawBackdrop()
        {
            /*Bitmap bmp = new Bitmap(176, 120);
            DrawBackdrop(bmp, 0, 0);
            return bmp;*/
            return null;
        }

        public void DrawBackdrop(Texture2D texture, int offsetX, int offsetY)
        {
            var colors = new Color[texture.Width * texture.Height];
            texture.GetData(colors);

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
                            colors[x * 8 + x2 + offsetX + (y * 8 + y2 + offsetY) * texture.Width] = col;
                        }
                }

            texture.SetData(colors);
        }
    }
}
