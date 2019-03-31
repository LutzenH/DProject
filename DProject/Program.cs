using System;
using System.Threading;
using Cairo;
using Gtk;

namespace DProject
{
    public class Program
    {   
        [STAThread]
        public static void Main(string[] args)
        {
            var window = new EditorWindow();
        }
    }
}
