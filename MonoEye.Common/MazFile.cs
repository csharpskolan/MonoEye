using MonoEye.Common;
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

    public class MazeBlock
    {
        public int North { get; private set; }
        public int West { get; private set; }
        public int South { get; private set; }
        public int East { get; private set; }

        private int[] _directions;

        public MazeBlock(int N, int W, int S, int E)
        {
            North = N;
            West = W;
            South = S;
            East = E;

            _directions = new[] { N, E, S, W };
        }

        public int GetFace(Vector2Int direction, int offset)
        {
            int index = 0;

            if (direction.X == 1)
                index = 1;
            else if (direction.Y == 1)
                index = 2;
            else if (direction.X == -1)
                index = 3;

            return _directions[(index + offset) % 4];
        }
    }
}
