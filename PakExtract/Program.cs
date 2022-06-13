using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

namespace PakExtract
{
    class Program
    {
        static void Main(string[] args)
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
            for(int i=0; i<vmpFile.NrOfWallTypes; i++)
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
