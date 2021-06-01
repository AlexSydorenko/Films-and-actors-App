using System;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;

namespace ConsoleApp
{
    public class FilmActorsRepository
    {
        private SqliteConnection connection;
        public FilmActorsRepository(string dbFilepath)
        {
            this.connection = new SqliteConnection($"Data Source = {dbFilepath}");
        }

        public long Insert(FilmActors filmActor)
        {
            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = 
            @"
                INSERT INTO filmActors (filmId, actorId)
                VALUES ($filmId, $actorId);
            
                SELECT last_insert_rowid();
            ";
            command.Parameters.AddWithValue("$filmId", filmActor.filmId);
            command.Parameters.AddWithValue("$actorId", filmActor.actorId);
            
            long newId = (long)command.ExecuteScalar();

            connection.Close();
        
            return newId;
        }

        public List<Actor> GetActorsFromTheFilm(int filmId)
        {
            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText =
            @"
                SELECT actors.id, fullName, age, residence
                FROM filmActors, actors
	                WHERE filmActors.filmId = $filmId AND actors.id = filmActors.actorId
            ";
            command.Parameters.AddWithValue("$filmId", filmId);
            
            SqliteDataReader reader = command.ExecuteReader();
            
            List<Actor> actors = new List<Actor>();
            while (reader.Read())
            {
                Actor actor = new Actor();
                actor.id = int.Parse(reader.GetString(0));
                actor.fullName = reader.GetString(1);
                actor.age = int.Parse(reader.GetString(2));
                actor.residence = reader.GetString(3);
                actors.Add(actor);
            }
            reader.Close();
            connection.Close();
            return actors;
        }

        public List<Film> GetActorFilms(int actorId)
        {
            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText =
            @"
                SELECT films.id, title, releaseYear, country, director
                FROM filmActors, films
	                WHERE filmActors.actorId = $actorId AND films.id = filmActors.filmId
            ";
            command.Parameters.AddWithValue("$actorId", actorId);
            
            SqliteDataReader reader = command.ExecuteReader();
            
            List<Film> films = new List<Film>();
            while (reader.Read())
            {
                Film film = new Film();
                film.id = int.Parse(reader.GetString(0));
                film.title = reader.GetString(1);
                film.releaseYear = int.Parse(reader.GetString(2));
                film.country = reader.GetString(3);
                film.director = reader.GetString(3);
                films.Add(film);
            }
            reader.Close();
            connection.Close();
            return films;
        }

        public bool DeleteFilm(int filmId)
        {
            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"DELETE FROM filmActors WHERE filmId = $filmId";
            command.Parameters.AddWithValue("$filmId", filmId);
            
            int nChanged = command.ExecuteNonQuery();
            
            if (nChanged == 0)
            {
                connection.Close();
                return false;
            }

            connection.Close();
            return true;
        }

        public bool DeleteActor(int actorId)
        {
            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"DELETE FROM filmActors WHERE actorId = $actorId";
            command.Parameters.AddWithValue("$actorId", actorId);
            
            int nChanged = command.ExecuteNonQuery();
            
            if (nChanged == 0)
            {
                connection.Close();
                return false;
            }

            connection.Close();
            return true;
        }
    }
}
