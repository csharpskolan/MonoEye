namespace MonoEye.Common
{
    public class PakFileInfo
    {
        public string FileName { get; set; }
        public int FileSize { get; set; }
        public int OffsetInPak { get; set; }
        public byte[] RawData { get; set; }
    }
}
