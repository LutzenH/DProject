using System;

namespace DProject
{
    public class Program
    {   
        [STAThread]
        public static void Main(string[] args)
        {
            var window = new EditorWindow(args);
        }
    }
}
