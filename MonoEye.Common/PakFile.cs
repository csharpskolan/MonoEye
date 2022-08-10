using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;

namespace MonoEye.Common
{
    public class PakFile
    {
        public ReadOnlyCollection<PakFileInfo> Files => _files.AsReadOnly();

        private List<PakFileInfo> _files;
        private byte[] _data;

        public PakFileInfo this[string index]
        {
            get
            {
                return Files.First(f => f.FileName == index);
            }
        }

        private PakFile(string fileName)
        {
            _files = new List<PakFileInfo>();
            _data = File.ReadAllBytes(fileName);

            // read header
            int offset = 0;
            int headerSize = _data[0] + (_data[1] << 8) +
                    (_data[2] << 16) + (_data[3] << 24);

            while (offset < headerSize - 4)
            {
                var info = new PakFileInfo();
                info.OffsetInPak = _data[offset] + (_data[offset + 1] << 8) +
                    (_data[offset + 2] << 16) + (_data[offset + 3] << 24);
                offset += 4;

                var nameData = new List<byte>();
                while (_data[offset] != 0)
                    nameData.Add(_data[offset++]);

                info.FileName = Encoding.UTF8.GetString(nameData.ToArray());
                offset++;

                int endOffset = _data[offset] + (_data[offset + 1] << 8) +
                    (_data[offset + 2] << 16) + (_data[offset + 3] << 24);
                info.FileSize = endOffset - info.OffsetInPak;

                //last entry?
                if (offset >= headerSize - 4)
                    info.FileSize = _data.Length - info.OffsetInPak;

                info.RawData = new byte[info.FileSize];
                Array.Copy(_data, info.OffsetInPak, info.RawData, 0, info.FileSize);

                _files.Add(info);
            }

        }

        public static PakFile FromFile(string fileName)
        {
            return new PakFile(fileName);
        }
    }
}
