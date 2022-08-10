using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace MonoEye.Common
{
    public class InfFile
    {
        public int TriggerOffset { get; private set; }
        public string MazeName { get; set; }
        public string VmpVcnName { get; set; }
        public string PaletteName { get; set; }
        public int Unknown { get; set; }
        public byte TypeOfCallingCommand1 { get; set; }
        public int Command1GenerationFrequencyInTicks { get; set; }
        public int Command1GenerationFrequencyInSteps { get; set; }
        public byte Monster1CompressionMethod { get; set; }
        public string Monster1Name { get; set; }
        public byte Monster2CompressionMethod { get; set; }
        public string Monster2Name { get; set; }
        public List<Monster> Monsters { get; set; }
        public int NrOfDecCommands { get; set; }
        public Dictionary<int, WallMapping> WallMappings { get; set; }

        private InfFile(byte[] data)
        {
            int index = 0;

            TriggerOffset = data[index++] + (data[index++] << 8);

            MazeName = Get12CharName(ref data, ref index);
            VmpVcnName = Get12CharName(ref data, ref index);
            PaletteName = Get12CharName(ref data, ref index);

            Unknown = data[index++] + (data[index++] << 8) + (data[index++] << 16) + (data[index++] << 24);
            TypeOfCallingCommand1 = data[index++];
            Command1GenerationFrequencyInTicks = data[index++] + (data[index++] << 8);
            Command1GenerationFrequencyInSteps = data[index++] + (data[index++] << 8);

            Monster1CompressionMethod = data[index++];
            Monster1Name = Get12CharName(ref data, ref index);
            Monster2CompressionMethod = data[index++];
            Monster2Name = Get12CharName(ref data, ref index);

            // skip 5 unknown bytes
            index += 5;

            // monsters 
            Monsters = new List<Monster>();
            for (int i = 0; i < 30; i++)
            {
                Monsters.Add(new Monster()
                {
                    Index = data[index++],
                    LevelType = data[index++],
                    Pos = data[index++] + (data[index++] << 8),
                    Subpos = data[index++],
                    Direction = data[index++],
                    Type = data[index++],
                    Picture = data[index++],
                    Phase = data[index++],
                    Pause = data[index++],
                    Weapon = data[index++] + (data[index++] << 8),
                    Pocket_item = data[index++] + (data[index++] << 8)
                });
            }

            NrOfDecCommands = data[index++] + (data[index++] << 8);
            Debug.WriteLine("NrOfDecCommands=" + NrOfDecCommands);
            WallMappings = new Dictionary<int, WallMapping>();
            string texture = string.Empty;

            for (int i = 0; i < NrOfDecCommands; i++)
            {
                var command = data[index++];

                switch (command)
                {
                    case 0xec:
                        Debug.WriteLine("0xec - Load overlay");
                        texture = Get12CharName(ref data, ref index);
                        Debug.WriteLine("gfx: " + texture);
                        Debug.WriteLine("rect: " + Get12CharName(ref data, ref index));
                        Debug.WriteLine("");
                        break;
                    case 0xfb:
                        Debug.WriteLine("0xfb - Wall mapping");
                        var wallMap = new WallMapping()
                        {
                            WallMappingIndex = data[index++],
                            WallID = data[index++],
                            DecorationID = data[index++],
                            WallType = data[index++],
                            WallPassabilityType = data[index++],
                            Texture = texture
                        };
                        WallMappings.Add(wallMap.WallMappingIndex, wallMap);
                        break;
                    default:
                        Debug.WriteLine("Unknown command:" + command);
                        Debug.WriteLine("");
                        break;
                }

            }

        }

        private string Get12CharName(ref byte[] data, ref int index)
        {
            var nameData = new List<byte>();
            bool nullReached = false;
            for (int i = 0; i < 12; i++)
            {
                if (data[index] == 0)
                    nullReached = true;
                if (!nullReached)
                    nameData.Add(data[index++]);
                else
                    index++;
            }

            return Encoding.UTF8.GetString(nameData.ToArray());
        }

        public static InfFile FromData(byte[] data)
        {
            return new InfFile(data);
        }
    }

    public struct WallMapping
    {
        public byte WallMappingIndex;
        public byte WallID;
        public byte DecorationID;
        public byte WallType;
        public byte WallPassabilityType;
        public string Texture;
    }

    public struct Monster
    {
        public byte Index;
        public byte LevelType;
        public int Pos;
        public byte Subpos;
        public byte Direction;
        public byte Type;
        public byte Picture;
        public byte Phase;
        public byte Pause;
        public int Weapon;
        public int Pocket_item;
    }
}