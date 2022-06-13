using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace MonoEye.Common
{
    public class ViewPortPositions
    {
        public int StepsForward { get; set; }
        public int StepsLeft { get; set; }
        public List<Texture2D> Texture { get; set; } = new List<Texture2D>();
        public Vector2 DrawPosition { get; set; }
        public int DirectionOffset { get; set; }
        public int DecorationOffset { get; set; }
        public int DecorationXDelta { get; set; }
    }
}
