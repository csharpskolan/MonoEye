using System;

namespace MonoEye.Game
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new MonoEyeGame())
                game.Run();
        }
    }
}
