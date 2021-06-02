using System;
using System.IO;
using Microsoft.Data.Sqlite;
using System.Text;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using Terminal.Gui;

namespace ConsoleApp
{
    class Program
    {
        static string databasePath = "/home/alex/projects/progbase3/data/database";

        static void Main(string[] args)
        {
            Application.Init();
            Toplevel top = Application.Top;

            MainWindow win = new MainWindow(databasePath);
            top.Add(win);

            Application.Run();
        }
    }
}
