using MonoEye.Common;

namespace MonoEye
{
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
