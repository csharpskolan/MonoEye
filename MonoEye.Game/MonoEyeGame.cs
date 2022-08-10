using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoEye.Common;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MonoEye.Game
{
    public class MonoEyeGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch _spriteBatch;

        private float _scale = 1.0f;
        private Vector2Int _position = new Vector2Int(10, 15);
        private Vector2Int _direction = new Vector2Int(0, -1);
        readonly List<ViewPortPositions> _viewPortPositions = new List<ViewPortPositions>();
        Texture2D _backDropTexture;
        Texture2D _playFieldTexture;
        SpriteFont _font;
        private MazFile _level;
        private InfFile _infFile;
        private DatFile _datFile;
        Dictionary<string, Texture2D> _decorationTextures;

        public MonoEyeGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Window.AllowUserResizing = true;
            Content.RootDirectory = "Content";

            Components.Add(new KeyboardComponent(this));
        }


        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 800;
            graphics.ApplyChanges();

            var pakFile = PakFile.FromFile("Paks\\EOBDATA3.PAK");
            var palette = PaletteFile.FromData(pakFile["BRICK.PAL"].RawData);
            var decodedBrick = CpsFile.FromData(pakFile["BRICK.VCN"].RawData);
            var vcnFile = VcnFile.FromData(decodedBrick.RawData);
            var vmpFile = VmpFile.FromData(pakFile["BRICK.VMP"].RawData);

            _level = MazFile.FromData(pakFile["LEVEL1.MAZ"].RawData);
            _datFile = DatFile.FromData(pakFile["BRICK.DAT"].RawData);
            _infFile = InfFile.FromData(CpsFile.FromData(pakFile["LEVEL1.INF"].RawData).RawData);

            var renderer = new BlockRenderer(vcnFile, vmpFile, palette);
            _backDropTexture = new Texture2D(GraphicsDevice, 176, 120);
            renderer.DrawBackdrop(_backDropTexture, 0, 0);

            int dataIndex = 0;
            foreach (var data in WallRenderData.Data)
            {
                dataIndex++;
                int xpos = data.OffsetInViewPort % 22 * 8;
                int ypos = data.OffsetInViewPort / 22 * 8;
                var vpp = new ViewPortPositions()
                {
                    DrawPosition = new Vector2(xpos, ypos),
                    StepsForward = data.StepsForward,
                    StepsLeft = data.StepsLeft,
                    DirectionOffset = data.DirectionOffset,
                    DecorationOffset = data.DecorationWallOffset,
                    DecorationXDelta = data.DecorationXDelta,
                    FlippedDecoration = data.FlipFlag
                };

                for (int i = 0; i < vmpFile.NrOfWallTypes; i++)
                {
                    var texture = new Texture2D(GraphicsDevice, data.VisibleWidthInBlocks * 8, data.VisibleHeightInBlocks * 8);
                    renderer.DrawWall(texture, i, data, -xpos, -ypos);

                    //Stream stream = File.Create($"vmp-{dataIndex}-type{i}.png");
                    //texture.SaveAsPng(stream, texture.Width, texture.Height);
                    //stream.Dispose();

                    vpp.Texture.Add(texture);
                }
                _viewPortPositions.Add(vpp);
            }

            _decorationTextures = new Dictionary<string, Texture2D>();

            foreach (var texName in _infFile.WallMappings.GroupBy(w => w.Value.Texture)
                .Select(grp => grp.First().Value.Texture))
            {
                var decodedImg = CpsFile.FromData(pakFile[texName.ToUpper() + ".CPS"].RawData);
                var texture = new Texture2D(GraphicsDevice, 320, 200);
                decodedImg.RenderTexture(texture, palette, true);
                _decorationTextures.Add(texName, texture);
            }

            pakFile = PakFile.FromFile("Paks\\EOBDATA5.PAK");
            var playField = CpsFile.FromData(pakFile["PLAYFLD.CPS"].RawData);
            pakFile = PakFile.FromFile("Paks\\EOBDATA2.PAK");
            palette = PaletteFile.FromData(pakFile["EOBPAL.COL"].RawData);
            _playFieldTexture = new Texture2D(GraphicsDevice, 320, 200);
            playField.RenderTexture(_playFieldTexture, palette, true);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _font = Content.Load<SpriteFont>("Font");
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            var nextPosition = _position;
            _scale = graphics.PreferredBackBufferWidth / 320f;

            if (KeyboardComponent.KeyPressed(Keys.Delete) || KeyboardComponent.KeyPressed(Keys.Q))
                nextPosition += _direction.Rotate90DegreesLeft();
            if (KeyboardComponent.KeyPressed(Keys.PageDown) || KeyboardComponent.KeyPressed(Keys.E))
                nextPosition += _direction.Rotate90DegreesRight();
            if (KeyboardComponent.KeyPressed(Keys.Up) || KeyboardComponent.KeyPressed(Keys.W))
                nextPosition += _direction;
            if (KeyboardComponent.KeyPressed(Keys.Down) || KeyboardComponent.KeyPressed(Keys.S))
                nextPosition -= _direction;
            if (KeyboardComponent.KeyPressed(Keys.Left) || KeyboardComponent.KeyPressed(Keys.A))
                _direction = _direction.Rotate90DegreesLeft();
            if (KeyboardComponent.KeyPressed(Keys.Right) || KeyboardComponent.KeyPressed(Keys.D))
                _direction = _direction.Rotate90DegreesRight();

            //if ((_level.Blocks.Index(nextPosition).North & 7) != (int)WallType.Wall1 &&
            //    (_level.Blocks.Index(nextPosition).North & 7) != (int)WallType.Wall2)
            _position = nextPosition;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend,
                SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);

            bool flipBackground = ((_position.X ^ _position.Y ^ _direction.X) & 1) > 0;
            _spriteBatch.Draw(_backDropTexture, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, _scale,
                flipBackground ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);

            foreach (var vp in _viewPortPositions)
            {
                var index = _position + vp.StepsForward * _direction + vp.StepsLeft * _direction.Rotate90DegreesLeft();
                if (!index.IsIndexable(_level.Width, _level.Height) || (_level.Blocks[index.X, index.Y].North & 7) == (int)WallType.Nothing)
                    continue;

                int wallType = _level.Blocks[index.X, index.Y].GetFace(_direction, vp.DirectionOffset);

                if (wallType > 6)
                {
                    // problem level 1 missing wallType 8 (?)
                    if (!_infFile.WallMappings.ContainsKey(wallType))
                        continue;

                    //wallmappaing
                    var mapping = _infFile.WallMappings[wallType];
                    var decId = mapping.DecorationID;

                    if (vp.DecorationOffset < 0)
                        continue;

                    if (mapping.WallID > 0)
                        _spriteBatch.Draw(vp.Texture[mapping.WallID - 1], vp.DrawPosition * _scale, null, Color.White, 0f, Vector2.Zero, _scale, SpriteEffects.None, 0f);

                    while (decId != 0 && decId != 0xFF)
                    {
                        var dec = _datFile.Decorations[decId];
                        var rectIndex = dec.RectangleIndices[vp.DecorationOffset];

                        if (rectIndex == 0xFF)
                            break;

                        var rect = _datFile.DecorationRectangles[rectIndex];
                        var x = dec.XCoords[vp.DecorationOffset];
                        var y = dec.YCoords[vp.DecorationOffset];

                        var calculatedX = x + vp.DecorationXDelta * 8;
                        var effect = SpriteEffects.None;

                        //decoration flipped?
                        if ((dec.Flags & 0x01) != 0)
                        {
                            calculatedX = calculatedX + rect.Width;
                            effect = SpriteEffects.FlipHorizontally;
                        }

                        //wall flipped?
                        if(vp.FlippedDecoration == 1)
                        {
                            calculatedX = 22*8 - calculatedX - rect.Width;
                            effect = SpriteEffects.FlipHorizontally;
                        }

                        _spriteBatch.Draw(_decorationTextures[mapping.Texture], new Vector2(calculatedX, y) * _scale,
                            new Rectangle(rect.X, rect.Y, rect.Width, rect.Height), Color.White, 0f, Vector2.Zero, _scale, effect, 0f);

                        decId = dec.LinkToNextDecoration;
                    }
                }
                else
                    _spriteBatch.Draw(vp.Texture[wallType - 1], vp.DrawPosition * _scale, null, Color.White, 0f, Vector2.Zero, _scale, SpriteEffects.None, 0f);
            }

            _spriteBatch.Draw(_playFieldTexture, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, _scale, SpriteEffects.None, 0f);

            var info = _level.Blocks.Index(_position);
            _spriteBatch.DrawString(_font, $"X={_position.X} Y={_position.Y}", new Vector2(5, 130) * _scale, Color.White);
            _spriteBatch.DrawString(_font, $"N={info.North} E={info.East} S={info.South} W={info.West}", new Vector2(5, 145) * _scale, Color.White);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
