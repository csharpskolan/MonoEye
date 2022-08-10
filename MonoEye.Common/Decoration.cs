namespace MonoEye.Common
{
    public class Decoration
    {
        public byte[] RectangleIndices;
        public byte LinkToNextDecoration;
        public byte Flags;
        public int[] XCoords;
        public int[] YCoords;

        public Decoration()
        {
            RectangleIndices = new byte[10];
            XCoords = new int[10];
            YCoords = new int[10];
        }
    }
}
