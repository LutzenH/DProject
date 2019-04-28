using System;

namespace DProject
{
    public class Program
    {   
        [STAThread]
        public static void Main(string[] args)
        {
#if EDITOR
            var window = new EditorWindow(args);
#else
            using (var game = new Game1())
                game.Run();
#endif
        }
    }
}
