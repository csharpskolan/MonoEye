using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace MonoEye.Common
{
    public class PaletteFile
    {
        public List<Color> Colors { get; set; }

        private PaletteFile(byte[] data)
        {
            Colors = new List<Color>();

            for (int i = 0; i < data.Length / 3; i++)
            {
                Colors.Add(new Color(
                    (byte)(data[i * 3] * 255 / 63),
                    (byte)(data[i * 3 + 1] * 255 / 63),
                    (byte)(data[i * 3 + 2] * 255 / 63),
                    (byte)255));
            }
        }

        public static PaletteFile FromData(byte[] data)
        {
            return new PaletteFile(data);
        }
    }
}
