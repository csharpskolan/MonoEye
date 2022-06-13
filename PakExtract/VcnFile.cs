using System.Collections.Generic;

namespace PakExtract
{
    class VcnFile
    {
        public int NrOfBlocks { get; private set; }
        public List<byte[,]> Blocks { get; set; }
        public byte[] BackdropLookup { get; set; }
        public byte[] WallLookup { get; set; }

        private VcnFile(byte[] data)
        {
            int index = 0;
            Blocks = new List<byte[,]>();
            NrOfBlocks = data[index++] + (data[index++] << 8);

            BackdropLookup = new byte[16];
            WallLookup = new byte[16];

            for (int i = 0; i < 16; i++)
                BackdropLookup[i] = data[index++];

            for (int i = 0; i < 16; i++)
                WallLookup[i] = data[index++];

            for(int i=0; i<NrOfBlocks; i++)
            {
                var block = new byte[8, 8];
                for(int x=0; x< 32; x++)
                {
                    var tmp = data[index++];
                    block[(x * 2) % 8, (x * 2) / 8] = (byte)(tmp >> 4);
                    block[((x * 2) % 8) + 1, (x * 2) / 8] = (byte)(tmp & 0xF);
                }

                Blocks.Add(block);
            }
        }

        public static VcnFile FromData(byte[] data)
        {
            return new VcnFile(data);
        }
    }
}
