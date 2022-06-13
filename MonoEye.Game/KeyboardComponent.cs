using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MonoEye.Game
{
    public class KeyboardComponent : GameComponent
    {
        public static KeyboardState CurrentState { get; set; }
        public static KeyboardState LastState { get; set; }

        private static Keys[] _inputKeys = { Keys.A, Keys.B, Keys.C, Keys.D, Keys.E, Keys.F, Keys.G, Keys.H,
                                             Keys.I, Keys.J, Keys.K, Keys.L, Keys.M, Keys.N, Keys.O, Keys.P, Keys.Q,
                                             Keys.R, Keys.S, Keys.T, Keys.U, Keys.V, Keys.W, Keys.X, Keys.Y, Keys.Z,
                                             Keys.D0, Keys.D1, Keys.D2, Keys.D3, Keys.D4, Keys.D5, Keys.D6, Keys.D7,
                                             Keys.D8, Keys.D9, Keys.Space };
        private static char[] _inputChars = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H',
                                              'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q',
                                              'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
                                              '0', '1', '2', '3', '4', '5', '6', '7',
                                              '8', '9', ' ' };

        public KeyboardComponent(Microsoft.Xna.Framework.Game game)
            : base(game)
        {
        }

        public override void Update(GameTime gameTime)
        {
            LastState = CurrentState;
            CurrentState = Keyboard.GetState();

            base.Update(gameTime);
        }

        public static bool KeyPressed(Keys key)
        {
            return CurrentState.IsKeyDown(key) && !LastState.IsKeyDown(key);
        }

        public static bool KeyUp(Keys key)
        {
            return CurrentState.IsKeyUp(key) && LastState.IsKeyDown(key);
        }

        public static string GetKeyInput()
        {
            string input = string.Empty;

            for (int i = 0; i < _inputKeys.Length; i++)
            {
                var key = _inputKeys[i];
                if (KeyPressed(key))
                    input += _inputChars[i];
            }

            return input;
        }
    }
}