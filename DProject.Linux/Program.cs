using System;
using DProject.Manager;

namespace DProject
{
    public class Program
    {   
        [STAThread]
        public static void Main(string[] args)
        {
            using (var game = new Game1(new GameEntityManager()))
                game.Run();
        }
    }
}
