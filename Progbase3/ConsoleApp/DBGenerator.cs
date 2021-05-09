using System;
using Microsoft.Data.Sqlite;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace ConsoleApp
{
    public class DBGenerator
    {
        public static void ProcessGeneration(SqliteConnection connection)
        {
            int numOfFilms = 0;
            Console.Write("Enter number of films to generate: ");
            if (!int.TryParse(Console.ReadLine(), out numOfFilms) || numOfFilms < 1)
            {
                Console.WriteLine("The number of films is positive integer! Try again!");
            }
            else
            {
                // Stopwatch sw = new Stopwatch();
                // sw.Start();
                GenerateFilmsAndActors(numOfFilms, connection);
                Console.WriteLine($"`{numOfFilms}` films and actors who starred in it were successfully added into the database!");
                // sw.Stop();
                // Console.WriteLine(sw.Elapsed);
            }

            // if (dataType == "films")
            // {
                
            // }
            // else if (dataType == "actors")
            // {
            //     int numOfActors = 0;
            //     Console.Write("Enter number of actors to generate: ");
            //     if (!int.TryParse(Console.ReadLine(), out numOfActors) || numOfActors < 1)
            //     {
            //         Console.WriteLine("The number of actors is positive integer! Try again!");
            //     }
            //     else
            //     {
            //         // Stopwatch sw = new Stopwatch();
            //         // sw.Start();
            //         GenerateActors(numOfActors, connection);
            //         Console.WriteLine($"`{numOfActors}` actors were successfully added into the database!");
            //         // sw.Stop();
            //         // Console.WriteLine(sw.Elapsed);
            //     }
            // }
            // else if (dataType == "reviews")
            // {
            //     int numOfReviews = 0;
            //     Console.Write("Enter number of reviews to generate: ");
            //     if (!int.TryParse(Console.ReadLine(), out numOfReviews) || numOfReviews < 1)
            //     {
            //         Console.WriteLine("The number of reviews is positive integer! Try again!");
            //     }
            //     else
            //     {
            //         // Stopwatch sw = new Stopwatch();
            //         // sw.Start();
            //         GenerateReviews(numOfReviews, connection);
            //         Console.WriteLine($"`{numOfReviews}` reviews were successfully added into the database!");
            //         // sw.Stop();
            //         // Console.WriteLine(sw.Elapsed);
            //     }
            // }
            // else
            // {
            //     Console.WriteLine($"Invalid data type: `{dataType}`!");
            // }
        }

        static void GenerateFilmsAndActors(int numOfFilms, SqliteConnection connection)
        {
            Random random = new Random();

            FilmRepository filmRepo = new FilmRepository(connection);
            ActorRepository actorRepo = new ActorRepository(connection);
            FilmActorsRepository filmActorsRepo = new FilmActorsRepository(connection);
            string s = "";
            // HashSet<int> uniqueNumbers = new HashSet<int>();
            while (numOfFilms > 0)
            {
                int randomNumber = random.Next(1, 2500);

                // while (uniqueNumbers.Contains(randomNumber))
                // {
                //     randomNumber = random.Next(1, 2500);
                // }
                // uniqueNumbers.Add(randomNumber);
                
                s = File.ReadLines("/home/alex/projects/progbase3/data/films.csv").Skip(randomNumber).First();
                string[] correctFormatFields = ProcessFilmInformation(s);

                Film film = new Film();
                film.title = correctFormatFields[0];
                film.releaseYear = int.Parse(correctFormatFields[1]);
                film.country = correctFormatFields[2];
                film.director = correctFormatFields[3];
                long filmId = filmRepo.Insert(film);

                // s = File.ReadLines("/home/alex/projects/progbase3/data/actors.csv").Skip(randomNumber).First();
                s = correctFormatFields[4];
                if (s.StartsWith('\"'))
                {
                    s = s.Substring(1, s.Length - 2);
                }
                string[] actors = s.Split(',');
                for (int i = 0; i < actors.Length; i++)
                {
                    if (actors[i].StartsWith(" "))
                    {
                        actors[i] = actors[i].Substring(1, actors[i].Length - 1);
                    }
                    Actor actor = new Actor();
                    actor.fullName = actors[i];
                    actor.age = random.Next(10, 90);
                    actor.residence = File.ReadLines("/home/alex/projects/progbase3/data/countries.csv").Skip(random.Next(1, 243)).First();

                    long actorId = 0;
                    if (actorRepo.GetByFullName(actor.fullName) != null)
                    {
                        actorId = actorRepo.GetByFullName(actor.fullName).id;
                    }
                    else
                    {
                        actorId = actorRepo.Insert(actor);
                    }

                    filmActorsRepo.Insert(new FilmActors(){filmId = filmId, actorId = actorId});
                    // numOfActors--;
                    // if (numOfActors == 0)
                    // {
                    //     return;
                    // }
                }

                numOfFilms--;
            }
        }

        // static void GenerateActors(int numOfActors, SqliteConnection connection)
        // {
        //     Random random = new Random();

        //     ActorRepository actorRepo = new ActorRepository(connection);
        //     string s = "";
        //     // HashSet<int> uniqueNumbers = new HashSet<int>();
        //     while (true)
        //     {
        //         int randomNumber = random.Next(1, 2500);

        //         // while (uniqueNumbers.Contains(randomNumber))
        //         // {
        //         //     randomNumber = random.Next(1, 2500);
        //         // }
        //         // uniqueNumbers.Add(randomNumber);
                
        //         s = File.ReadLines("/home/alex/projects/progbase3/data/actors.csv").Skip(randomNumber).First();
        //         if (s.StartsWith('\"'))
        //         {
        //             s = s.Substring(1, s.Length - 2);
        //         }
        //         string[] substrings = s.Split(',');
        //         foreach (string item in substrings)
        //         {
        //             Actor actor = new Actor();
        //             actor.fullName = item;
        //             actor.age = random.Next(10, 90);
        //             actor.residence = File.ReadLines("/home/alex/projects/progbase3/data/countries.csv").Skip(random.Next(1, 243)).First();
        //             actorRepo.Insert(actor);
        //             numOfActors--;
        //             if (numOfActors == 0)
        //             {
        //                 return;
        //             }
        //         }
        //     }
        // }

        // static void GenerateReviews(int numOfReviews, SqliteConnection connection)
        // {
        //     Random random = new Random();

        //     ReviewRepository reviewRepo = new ReviewRepository(connection);
        //     string s = "";
        //     // HashSet<int> uniqueNumbers = new HashSet<int>();
        //     while (numOfReviews > 0)
        //     {
        //         int randomNumber = random.Next(1, 2500);

        //         // while (uniqueNumbers.Contains(randomNumber))
        //         // {
        //         //     randomNumber = random.Next(1, 2500);
        //         // }
        //         // uniqueNumbers.Add(randomNumber);
                
        //         s = File.ReadLines("/home/alex/projects/progbase3/data/films.csv").Skip(randomNumber).First();
        //         string[] substrings = s.Split(',');
        //         string[] correctFormatFields = new string[7];
        //         int index = 0;
        //         for (int i = 0; i < substrings.Length; i++)
        //         {
        //             string valueInCorrectFormat = "";
        //             if (substrings[i].StartsWith("\""))
        //             {
        //                 while (true)
        //                 {
        //                     if (substrings[i][substrings[i].Length-1] == '\"')
        //                     {
        //                         valueInCorrectFormat += substrings[i];
        //                         break;
        //                     }
        //                     valueInCorrectFormat += substrings[i] + ",";
        //                     i++;
        //                 }
        //                 string stringCopy = valueInCorrectFormat;
        //                 valueInCorrectFormat = "";
        //                 for (int j = 1; j < stringCopy.Length - 1; j++)
        //                 {
        //                     valueInCorrectFormat += stringCopy[j];
        //                 }
        //             }
        //             else
        //             {
        //                 valueInCorrectFormat = substrings[i];
        //             }
        //             correctFormatFields[index] = valueInCorrectFormat;
        //             index++;
        //         }
        //         Review review = new Review();
        //         review.text = correctFormatFields[5];
        //         review.rating = double.Parse(correctFormatFields[6]);
        //         reviewRepo.Insert(review);

        //         numOfReviews--;
        //     }
        // }

        static string[] ProcessFilmInformation(string str)
        {
            string[] substrings = str.Split(',');
            string[] correctFormatFields = new string[7];
            int index = 0;
            for (int i = 0; i < substrings.Length; i++)
            {
                string valueInCorrectFormat = "";
                if (substrings[i].StartsWith("\""))
                {
                    while (true)
                    {
                        if (substrings[i][substrings[i].Length-1] == '\"')
                        {
                            valueInCorrectFormat += substrings[i];
                            break;
                        }
                        valueInCorrectFormat += substrings[i] + ",";
                        i++;
                    }
                    string stringCopy = valueInCorrectFormat;
                    valueInCorrectFormat = "";
                    for (int j = 1; j < stringCopy.Length - 1; j++)
                    {
                        valueInCorrectFormat += stringCopy[j];
                    }
                }
                else
                {
                    valueInCorrectFormat = substrings[i];
                }
                correctFormatFields[index] = valueInCorrectFormat;
                index++;
            }

            return correctFormatFields;
        }
    }
}
