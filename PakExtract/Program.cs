using MonoEye.Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

namespace PakExtract
{
    class Program
    {
        static void Main(string[] args)
        {
            //RenderWallsTest();
            RenderDecorationsTest();
        }

        static void RenderDecorationsTest()
        {
            var pakFile = PakFile.FromFile("EOBDATA3.PAK");
            var palette = PaletteFile.FromData(pakFile["BRICK.PAL"].RawData);
            var decodedBrick = CpsFile.FromData(pakFile["BRICK.VCN"].RawData);
            var vcnFile = VcnFile.FromData(decodedBrick.RawData);
            var vmpFile = VmpFile.FromData(pakFile["BRICK.VMP"].RawData);
            var renderer = new BlockRenderer(vcnFile, vmpFile, palette);

            var _infFile = InfFile.FromData(CpsFile.FromData(pakFile["LEVEL1.INF"].RawData).RawData);
            var _datFile = DatFile.FromData(pakFile["BRICK.DAT"].RawData);
            var textures = new Dictionary<string, Bitmap>();

            var pens = new Pen[] { Pens.Green, Pens.Maroon, Pens.Magenta, Pens.PeachPuff, Pens.PapayaWhip, Pens.PaleTurquoise };
            Font drawFont = new Font("Courier New", 8);

            foreach (var texName in _infFile.WallMappings.GroupBy(w => w.Value.Texture)
                .Select(grp => grp.First().Value.Texture))
            {
                var decodedImg = CpsFile.FromData(pakFile[texName.ToUpper() + ".CPS"].RawData);
                textures.Add(texName, decodedImg.GetBitmap(palette));
            }

            Console.WriteLine("LEVEL 1 wallmappings");
            int count = 0;

            foreach (var keyValuePairmapping in _infFile.WallMappings)
            {
                var decID = keyValuePairmapping.Value.DecorationID;
                Console.WriteLine($"index {keyValuePairmapping.Key} - texture {keyValuePairmapping.Value.Texture} - decorationId {decID}");

                if(decID >= _datFile.Decorations.Count)
                {
                    Console.WriteLine($"skipping decoration id {decID}");
                    continue;
                }

                var decoration = _datFile.Decorations[decID];
                Console.WriteLine($"flags - {decoration.Flags} - linktoNext {decoration.LinkToNextDecoration}");

                bool drawnId = false;
                for (int i = 0; i < 10; i++)
                {
                    var index = decoration.RectangleIndices[i];
                    if (index == 255)
                    {
                        //Console.WriteLine($"skipping pos {i}");
                        continue;
                    }
                    var rect = _datFile.DecorationRectangles[index];
                    var x = decoration.XCoords[i];
                    var y = decoration.YCoords[i];

                    var bmp = textures[keyValuePairmapping.Value.Texture];
                    using (var g = Graphics.FromImage(bmp))
                    {
                        if (!drawnId)
                        {
                            g.DrawString(decID.ToString(), drawFont, pens[count % pens.Count()].Brush, new PointF(rect.X, rect.Y));
                            drawnId = true;
                        }
                        g.DrawRectangle(pens[count % pens.Count()], new Rectangle(rect.X, rect.Y, rect.Width, rect.Height));
                    }
                }
                count++;
            }

            foreach(var valuePair in textures)
            {
                valuePair.Value.Save(valuePair.Key + ".png");
            }

            Console.WriteLine("done!");
            Console.ReadKey();
        }

        static void RenderWallsTest()
        {
            var pakFile = PakFile.FromFile("EOBDATA3.PAK");
            var palette = PaletteFile.FromData(pakFile["BRICK.PAL"].RawData);
            var decodedBrick = CpsFile.FromData(pakFile["BRICK.VCN"].RawData);
            var vcnFile = VcnFile.FromData(decodedBrick.RawData);
            var vmpFile = VmpFile.FromData(pakFile["BRICK.VMP"].RawData);

            var renderer = new BlockRenderer(vcnFile, vmpFile, palette);
            var bmp = new Bitmap(176 * 5, 120 * 5);

            var desc = new string[]
            {
                "A-east ",
                "B-east ",
                "C-east ",
                "E-west ",
                "F-west ",
                "G-west ",
                "B-south",
                "C-south",
                "D-south",
                "E-south",
                "F-south",
                "H-east ",
                "I-east ",
                "K-west ",
                "L-west ",
                "I-south",
                "J-south",
                "K-south",
                "M-east ",
                "O-west ",
                "M-south",
                "O-south",
                "N-south",
                "P-east ",
                "Q-west "
            };

            for (int i = 0; i <= 24; i++)
            {
                int offsetX = (i % 5) * 176;
                int offsetY = (i / 5) * 120;

                renderer.DrawBackdrop(bmp, offsetX, offsetY);
                renderer.DrawWall(bmp, 0, i, offsetX, offsetY);
                Graphics g = Graphics.FromImage(bmp);
                g.DrawString(desc[i], new Font("Tahoma", 8), Brushes.White, 0 + offsetX, 105 + offsetY);
                g.Flush();
            }
            bmp.Save("wall0_all_fix.png", ImageFormat.Png);

            bmp = new Bitmap(176 * 3, 120 * 2);
            for (int i = 0; i < vmpFile.NrOfWallTypes; i++)
            {
                int offsetX = (i % 3) * 176;
                int offsetY = (i / 3) * 120;
                renderer.DrawBackdrop(bmp, offsetX, offsetY);
                renderer.DrawWall(bmp, i, 22, offsetX, offsetY);
            }
            bmp.Save("wall_all_types.png", ImageFormat.Png);

            Console.WriteLine("Done!");
            Console.ReadKey();
        }

        
    }
}
