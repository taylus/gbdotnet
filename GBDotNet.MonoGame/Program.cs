using System;

namespace MonoGameBoy
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new MonoGameBoy())
                game.Run();
        }
    }
}
