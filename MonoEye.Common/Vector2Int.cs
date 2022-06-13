namespace MonoEye.Common
{
    public struct Vector2Int
    {
        public int X;
        public int Y;

        public Vector2Int(int x, int y)
        {
            X = x;
            Y = y;
        }

        #region Equals

        public override bool Equals(object obj)
        {
            if (obj is Vector2Int) return Equals((Vector2Int)obj);
            else return false;
        }

        public bool Equals(Vector2Int other)
        {
            return X == other.X && Y == other.Y;
        }

        #endregion

        #region Operators

        public static bool operator ==(Vector2Int value1, Vector2Int value2)
        {
            return value1.X == value2.X && value1.Y == value2.Y;
        }

        public static Vector2Int operator +(Vector2Int value1, Vector2Int value2)
        {
            return new Vector2Int(value1.X + value2.X, value1.Y + value2.Y);
        }

        public static Vector2Int operator -(Vector2Int value1, Vector2Int value2)
        {
            return new Vector2Int(value1.X - value2.X, value1.Y - value2.Y);
        }

        public static Vector2Int operator *(Vector2Int value1, int value2)
        {
            return new Vector2Int(value1.X * value2, value1.Y * value2);
        }

        public static Vector2Int operator *(int value1, Vector2Int value2)
        {
            return new Vector2Int(value1 * value2.X, value1 * value2.Y);
        }

        public static bool operator !=(Vector2Int value1, Vector2Int value2)
        {
            if (value1.X == value2.X) return value1.Y != value2.Y;
            return true;
        }

        #endregion

        #region Overrides

        public override int GetHashCode()
        {
            return X.GetHashCode() + Y.GetHashCode();
        }

        public override string ToString()
        {
            return $"{{X:{X} Y:{Y}}}";
        }

        #endregion

        #region Methods

        public Vector2Int Rotate90DegreesRight()
        {
            return new Vector2Int(-Y, X);
        }

        public Vector2Int Rotate90DegreesLeft()
        {
            return new Vector2Int(Y, -X);
        }

        public bool IsIndexable(int mapWidth, int mapHeight)
        {
            return X >= 0 && Y >= 0 && X < mapWidth && Y < mapHeight;
        }

        #endregion
    }
}
