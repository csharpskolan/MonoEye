namespace PakExtract
{
    class VmpFile
    {
        public int Header { get; set; }
        public int[,] BackgroundTiles { get; set; }

        public int NrOfWallTypes { get; set; }
        public int[,] WallTiles { get; set; }

        private VmpFile(byte[] data)
        {
            int index = 0;
            Header = data[index++] + (data[index++] << 8);
            BackgroundTiles = new int[22, 15];

            for(int y=0; y<15; y++)
                for(int x=0; x<22; x++)
                    BackgroundTiles[x, y] = data[index++] + (data[index++] << 8);

            NrOfWallTypes = (data.Length - 2) / (431 * 2);
            WallTiles = new int[NrOfWallTypes, 431];

            for(int i =0; i<NrOfWallTypes; i++)
                for(int c=0; c<431; c++)
                    WallTiles[i, c] = data[index++] + (data[index++] << 8);
        }

        public static VmpFile FromData(byte[] data)
        {
            return new VmpFile(data);
        }
    }
}
