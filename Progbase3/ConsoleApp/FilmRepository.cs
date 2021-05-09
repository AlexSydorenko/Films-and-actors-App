using System;
using Microsoft.Data.Sqlite;

namespace ConsoleApp
{
    public class FilmRepository
    {
        private SqliteConnection connection;
        public FilmRepository(SqliteConnection connection)
        {
            this.connection = connection;
        }

        public long Insert(Film film)
        {
            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = 
            @"
                INSERT INTO films (title, releaseYear, country, director)
                VALUES ($title, $releaseYear, $country, $director);
            
                SELECT last_insert_rowid();
            ";
            command.Parameters.AddWithValue("$title", film.title);
            command.Parameters.AddWithValue("$releaseYear", film.releaseYear);
            command.Parameters.AddWithValue("$country", film.country);
            command.Parameters.AddWithValue("$director", film.director);
            
            long newId = (long)command.ExecuteScalar();

            connection.Close();
        
            return newId;
        }

        public Film GetById(int id)
        {
            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM films WHERE id = $id";
            command.Parameters.AddWithValue("$id", id);
            
            SqliteDataReader reader = command.ExecuteReader();
            
            Film film = null;
            if (reader.Read())
            {
                film = new Film();
                film.id = int.Parse(reader.GetString(0));
                film.title = reader.GetString(1);
                film.releaseYear = int.Parse(reader.GetString(2));
                film.country = reader.GetString(3);
                film.director = reader.GetString(4);
            }
            reader.Close();
            connection.Close();

            return film;
        }

        public Film GetByTitle(string title)
        {
            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM films WHERE title = $title";
            command.Parameters.AddWithValue("$title", title);
            
            SqliteDataReader reader = command.ExecuteReader();
            
            Film film = null;
            if (reader.Read())
            {
                film = new Film();
                film.id = int.Parse(reader.GetString(0));
                film.title = reader.GetString(1);
                film.releaseYear = int.Parse(reader.GetString(2));
                film.country = reader.GetString(3);
                film.director = reader.GetString(4);
            }
            reader.Close();
            connection.Close();

            return film;
        }

        public bool Delete(int id)
        {
            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"DELETE FROM films WHERE id = $id";
            command.Parameters.AddWithValue("$id", id);
            
            int nChanged = command.ExecuteNonQuery();
            
            if (nChanged == 0)
            {
                return false;
            }

            connection.Close();

            return true;
        }

        public bool Update(int id, Film film)
        {
            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText =
            @"
                UPDATE films
                    SET title = $title, releaseYear = $releaseYear, country = $country, director = $director
                    WHERE id = $id
            ";
            command.Parameters.AddWithValue("$id", id);
            command.Parameters.AddWithValue("$title", film.title);
            command.Parameters.AddWithValue("$releaseYear", film.releaseYear);
            command.Parameters.AddWithValue("$country", film.country);
            command.Parameters.AddWithValue("$director", film.director);

            int nChanged = command.ExecuteNonQuery();

            if (nChanged == 0)
            {
                return false;
            }

            connection.Close();
            
            return true;
        }
    }
}
