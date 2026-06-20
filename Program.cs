using System;

namespace Mario
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new MarioGame())
                game.Run();
        }
    }
}
