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
            // string databasePath = "/home/alex/projects/progbase3/data/database";
            // SqliteConnection connection = new SqliteConnection($"Data Source={databasePath}");

            Application.Init();
            Toplevel top = Application.Top;

            MainWindow win = new MainWindow(databasePath);
            top.Add(win);

            Application.Run();

            // DBGenerator.ProcessGeneration(connection);
            
            // UserRepository userRepo = new UserRepository(connection);
            // FilmRepository filmRepo = new FilmRepository(connection);
            // ActorRepository actorRepo = new ActorRepository(connection);
            // ReviewRepository reviewRepo = new ReviewRepository(connection);
            // FilmActorsRepository filmActorsRepo = new FilmActorsRepository(connection);


            // IMPORT / EXPORT TESTING //

            // DataExportAndImport.ExportFilmReviews(reviewRepo, 2, "./export.xml");
            // DataExportAndImport.ImportFilmReviews(reviewRepo, 1, "./export.xml");
            // foreach (Review review in filmReviews)
            // {
            //     Console.WriteLine(review);
            // }

            // User currentUser = new User();
            // currentUser.id = 1;
            // Film film = filmRepo.GetById(2);
            // string reviewText = "It's my review!";
            // int rating = 8;
            // Review review = new Review(){
            //     text = reviewText,
            //     rating = rating,
            //     film = film,
            //     author = currentUser,
            // };
            // long reviewId = reviewRepo.Insert(review);
            // Console.WriteLine($"review {reviewId} added!");

            

            // List<Film> films = filmActorsRepo.GetActorFilms(3);
            // foreach (Film film in films)
            // {
            //     Console.WriteLine(film.ToString());
            //     Console.WriteLine();
            // }

            // List<Actor> actors = filmActorsRepo.GetActorsFromTheFilm(2);
            // foreach (Actor actor in actors)
            // {
            //     Console.WriteLine(actor.ToString());
            // }


            // FilmActorsRepository filmActorsRepo = new FilmActorsRepository(connection);
            // filmActorsRepo.Insert(new FilmActors(){
            //     filmId = 10,
            //     actorId = 34,
            // });

            
            // List<Review> authorReviews = reviewRepo.GetAllFilmReviews(2);
            // foreach (Review r in authorReviews)
            // {
            //     Console.WriteLine(r.ToString());
            //     Console.WriteLine();
            // }
        }
    }
}
