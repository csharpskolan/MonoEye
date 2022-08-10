using System.Collections.Generic;
using System.Text;

namespace MonoEye.Common
{
    public class DatFile
    {
        public int NrOfDecorations { get; set; }
        public List<Decoration> Decorations { get; set; }
        public int NrOfDecorationRectangles { get; set; }
        public List<DecorationRectangle> DecorationRectangles { get; set; }

        private DatFile(byte[] data)
        {
            int index = 0;

            Decorations = new List<Decoration>();
            NrOfDecorations = data[index++] + (data[index++] << 8);

            for (int i = 0; i < NrOfDecorations; i++)
            {
                var decoration = new Decoration();
                for (int j = 0; j < 10; j++)
                    decoration.RectangleIndices[j] = data[index++];
                decoration.LinkToNextDecoration = data[index++];
                decoration.Flags = data[index++];
                for (int j = 0; j < 10; j++)
                    decoration.XCoords[j] = data[index++] + (data[index++] << 8);
                for (int j = 0; j < 10; j++)
                    decoration.YCoords[j] = data[index++] + (data[index++] << 8);

                Decorations.Add(decoration);
            }

            NrOfDecorationRectangles = data[index++] + (data[index++] << 8);
            DecorationRectangles = new List<DecorationRectangle>();

            for (int i = 0; i < NrOfDecorationRectangles; i++)
            {
                var decorationRectangle = new DecorationRectangle();
                decorationRectangle.X = (data[index++] + (data[index++] << 8)) * 8;
                decorationRectangle.Y = data[index++] + (data[index++] << 8);
                decorationRectangle.Width = (data[index++] + (data[index++] << 8)) * 8;
                decorationRectangle.Height = data[index++] + (data[index++] << 8);

                DecorationRectangles.Add(decorationRectangle);
            }
        }

        public static DatFile FromData(byte[] data)
        {
            return new DatFile(data);
        }
    }
}
