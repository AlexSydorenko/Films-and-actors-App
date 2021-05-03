using System;
using System.IO;
using Microsoft.Data.Sqlite;
using System.Text;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            string databasePath = "/home/alex/projects/progbase3/data/database";
            SqliteConnection connection = new SqliteConnection($"Data Source={databasePath}");

            DBGenerator.ProcessGeneration(connection);
        }
    }
}
