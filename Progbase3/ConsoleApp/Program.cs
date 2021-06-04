using System;
using System.IO;
using Terminal.Gui;

namespace ConsoleApp
{
    class Program
    {
        static string databasePath = "/home/alex/projects/progbase3/data/database";

        static void Main(string[] args)
        {
            if (!File.Exists(databasePath))
            {
                Console.WriteLine("Program could not be run! Database file was replaced!");
            }
            else
            {
                Application.Init();
                Toplevel top = Application.Top;

                MainWindow win = new MainWindow(databasePath);
                top.Add(win);

                Application.Run();
            }
        }
    }
}
