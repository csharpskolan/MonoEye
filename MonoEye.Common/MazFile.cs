using System.Collections.Generic;

namespace MonoEye
{
    public class MazFile
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int NrOfFaces { get; private set; }
        public MazeBlock[,] Blocks { get; set; }

        private MazFile(byte[] data)
        {
            int index = 0;

            Width = data[index++] + (data[index++] << 8);
            Height = data[index++] + (data[index++] << 8);
            NrOfFaces = data[index++] + (data[index++] << 8);
            Blocks = new MazeBlock[Width, Height];

            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                {
                    Blocks[x, y] = new MazeBlock(data[index++], data[index++], data[index++], data[index++]);
                }
        }

        public static MazFile FromData(byte[] data)
        {
            return new MazFile(data);
        }
    }
}
