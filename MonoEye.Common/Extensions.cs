namespace MonoEye.Common
{
    public static class Extensions
    {
        public static int Index(this int[,] array, Vector2Int index)
        {
            return array[index.X, index.Y];
        }

        public static MazeBlock Index(this MazeBlock[,] array, Vector2Int index)
        {
            return array[index.X, index.Y];
        }
    }
}
