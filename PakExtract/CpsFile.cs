using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;

namespace PakExtract
{
    enum CompressionType
    {
        Uncompressed = 0,
        WestwoodLZW12 = 1,
        WestwoodLZW14 = 2,
        WestwoodRLE = 3,
        WestwoodLCW = 4
    }
    class CpsFile
    {
        private byte[] _data;

        public int FileSize { get; private set; }
        public CompressionType CompressionType { get; private set; }
        public int UncompressedSize { get; set; }
        public int PaletteSize { get; set; }

        public bool IsRelative => _data[10] == 0;
        public byte[] RawData { get; set; }

        private CpsFile(byte[] data)
        {
            _data = data;
            int index = 0;

            FileSize = _data[index++] + (_data[index++] << 8);
            CompressionType = (CompressionType)(_data[index++] + (_data[index++] << 8));
            UncompressedSize = _data[index++] + (_data[index++] << 8) + (_data[index++] << 16) + (_data[index++] << 24);
            PaletteSize = _data[index++] + (_data[index++] << 8);
            RawData = new byte[UncompressedSize];

            if (IsRelative)
                index++;

            int destIndex = 0;

            while (index < _data.Length)
            {
                switch (_data[index++])
                {
                    // Command 1: Short copy
                    case byte c when ((c >> 6) == 2):
                        int count = c & 0x3F;
                        for (int i = 0; i < count; i++)
                            RawData[destIndex++] = _data[index++];

                        //Debug.WriteLine($"Command 1 - count {count}");
                        break;

                    // Command 2: Existing block relative copy
                    case byte c when ((c >> 7) == 0):
                        count = (c >> 4) + 3;
                        int pos = _data[index++] + ((c & 0xF) << 8);
                        int destPos = destIndex - pos;

                        for (int i = 0; i < count; i++)
                            RawData[destIndex++] = RawData[destPos++];

                        //Debug.WriteLine($"Command 2 - count {count} pos {pos}");
                        break;

                    // Command 5: Existing block long copy
                    case byte c when (c == 255):
                        count = _data[index++] + (_data[index++] << 8);
                        pos = _data[index++] + (_data[index++] << 8);
                        destPos = (IsRelative) ? destIndex - pos : pos;

                        for (int i = 0; i < count; i++)
                            RawData[destIndex++] = RawData[destPos++];

                        //Debug.WriteLine($"Command 5 - count {count} pos {pos}");
                        break;

                    // Command 4: Repeat value
                    case byte c when (c == 254):
                        count = _data[index++] + (_data[index++] << 8);
                        byte val = _data[index++];

                        for (int i = 0; i < count; i++)
                            RawData[destIndex++] = val;

                        //Debug.WriteLine($"Command 4 - count {count} value x{val:X2}");
                        break;

                    // Command 3: Existing block medium-length copy
                    case byte c when ((c >> 6) == 3):
                        count = (c & 0x3F) + 3;
                        pos = _data[index++] +(_data[index++] << 8);
                        destPos = (IsRelative) ? destIndex - pos : pos;

                        for (int i = 0; i < count; i++)
                            RawData[destIndex++] = RawData[destPos++];

                        //Debug.WriteLine($"Command 3 - count {count} pos {pos}");
                        break;
                }
            }
        }

        public void SaveAsPng(string filename, PaletteFile palette, bool transparent = false)
        {
            Bitmap bmp = new Bitmap(320, 200);

            for (int i = 0; i < RawData.Length; i++)
            {
                if(transparent && RawData[i] == 0)
                    bmp.SetPixel(i % 320, i / 320, Color.Transparent);
                else
                    bmp.SetPixel(i % 320, i / 320, palette.Colors[RawData[i]]);

            }
            bmp.Save(filename, ImageFormat.Png);
        }

        public static CpsFile FromData(byte[] data)
        {
            return new CpsFile(data);
        }

        public static bool IsCps(byte[] data)
        {
            return data[data.Length - 1] == 0x80;
        }
    }
}
