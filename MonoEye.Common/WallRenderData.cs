using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MonoEye.Common
{
    public struct WallRenderData
    {
        public short BaseOffset;
        public short OffsetInViewPort;
        public short VisibleHeightInBlocks;
        public short VisibleWidthInBlocks;
        public short SkipValue;
        public short FlipFlag;
        public int StepsForward;
        public int StepsLeft;
        public int DirectionOffset;
        public int DecorationWallOffset;
        public int DecorationXDelta;

        public WallRenderData(short bo, short op, short vH,
            short vW, short skip, short flip, int sf, int sl, int dO, int dWO, int dXD)
        {
            BaseOffset = bo;
            OffsetInViewPort = op;
            VisibleWidthInBlocks = vW;
            VisibleHeightInBlocks = vH;
            SkipValue = skip;
            FlipFlag = flip;
            StepsForward = sf;
            StepsLeft = sl;
            DirectionOffset = dO;
            DecorationWallOffset = dWO;
            DecorationXDelta = dXD;
        }

        public static readonly IList<WallRenderData> Data =
            new ReadOnlyCollection<WallRenderData>(new[]
            {
                new WallRenderData(104, 66, 5, 1, 2, 0, 3, 3, 1,-1,  0),/* A-east */
                new WallRenderData(102, 68, 5, 3, 0, 0, 3, 2, 1, 9,  0),/* B-east */
                new WallRenderData(97,  74, 5, 1, 0, 0, 3, 1, 1, 7,  0),/* C-east */
                new WallRenderData(97,  79, 5, 1, 0, 1, 3,-1, 3, 7,  0),/* E-west */
                new WallRenderData(102, 83, 5, 3, 0, 1, 3,-2, 3, 9,  0),/* F-west */
                new WallRenderData(104, 87, 5, 1, 2, 1, 3,-3, 3,-1,  0),/* G-west */
                new WallRenderData(133, 66, 5, 2, 4, 0, 3, 2, 2, 3,-12),/* B-south */
                new WallRenderData(129, 68, 5, 6, 0, 0, 3, 1, 2, 3, -6),/* C-south */
                new WallRenderData(129, 74, 5, 6, 0, 0, 3, 0, 2, 3,  0),/* D-south */
                new WallRenderData(129, 80, 5, 6, 0, 0, 3,-1, 2, 3,  6),/* E-south */
                new WallRenderData(129, 86, 5, 2, 4, 0, 3,-2, 2, 3, 12),/* F-south */
                new WallRenderData(117, 66, 6, 2, 0, 0, 2, 2, 1, 8,  0),/* H-east */
                new WallRenderData(81,  50, 8, 2, 0, 0, 2, 1, 1, 6,  0),/* I-east */
                new WallRenderData(81,  58, 8, 2, 0, 1, 2,-1, 3, 6,  0),/* K-west */
                new WallRenderData(117, 86, 6, 2, 0, 1, 2,-2, 3, 8,  0),/* L-west */
                new WallRenderData(163, 44, 8, 6, 4, 0, 2, 1, 2, 2,-10),/* I-south */
                new WallRenderData(159, 50, 8,10, 0, 0, 2, 0, 2, 2,  0),/* J-south */
                new WallRenderData(159, 60, 8, 6, 4, 0, 2,-1, 2, 2, 10),/* K-south */
                new WallRenderData(45,  25,12, 3, 0, 0, 1, 1, 1, 5,  0),/* M-east */
                new WallRenderData(45,  38,12, 3, 0, 1, 1,-1, 3, 5,  0), /* O-west */
                new WallRenderData(252, 22,12, 3,13, 0, 1, 1, 2, 1,-16),/* M-south */
                new WallRenderData(239, 41,12, 3,13, 0, 1,-1, 2, 1, 16),/* O-south */
                new WallRenderData(239, 25,12,16, 0, 0, 1, 0, 2, 1,  0),/* N-south */
                new WallRenderData(0,    0,15, 3, 0, 0, 0, 1, 1, 4,  0),/* P-east */
                new WallRenderData(0,   19,15, 3, 0, 1, 0,-1, 3, 4,  0),/* Q-west */
            });
    }
}
