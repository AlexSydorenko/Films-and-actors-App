using System;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;

namespace ConsoleApp
{
    public class FilmRepository
    {
        private SqliteConnection connection;
        public FilmRepository(string dbFilepath)
        {
            this.connection = new SqliteConnection($"Data Source = {dbFilepath}");
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

        private List<Film> SearchFilms(string searchValue)
        {
            List<Film> searchFilms = new List<Film>();
            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM films
                WHERE title LIKE '%' || $searchValue || '%' OR releaseYear LIKE '%' || $searchValue || '%'";

            command.Parameters.AddWithValue("$searchValue", searchValue);
            SqliteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                Film film = new Film();
                film.id = int.Parse(reader.GetString(0));
                film.title = reader.GetString(1);
                film.releaseYear = int.Parse(reader.GetString(2));
                film.country = reader.GetString(3);
                film.director = reader.GetString(4);
                searchFilms.Add(film);
            }
            connection.Close();

            return searchFilms;
        }

        public int GetTotalPages(int pageSize)
        {
            if (pageSize < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(pageSize));
            }

            return (int)Math.Ceiling(this.GetCount() / (double)pageSize);
        }

        public int GetSearchPagesCount(string searchValue, int pageSize)
        {
            if (pageSize < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(pageSize));
            }

            if (string.IsNullOrEmpty(searchValue))
            {
                return this.GetTotalPages(pageSize);
            }
            return (int)Math.Ceiling(this.SearchFilms(searchValue).Count / (double)pageSize);
        }

        private long GetCount()
        {
            connection.Open();
        
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT COUNT(*) FROM films";
            
            long count = (long)command.ExecuteScalar();

            connection.Close();
            return count;
        }

        public List<Film> GetSearchPage(string searchValue, int pageNum, int pageSize)
        {
            if (string.IsNullOrEmpty(searchValue))
            {
                return this.GetPage(pageNum, pageSize);
            }

            if (pageNum < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(pageNum));
            }

            if (pageSize < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(pageSize));
            }

            List<Film> searchFilms = this.SearchFilms(searchValue);

            List<Film> page = new List<Film>();
            int offset = (pageNum - 1) * pageSize;
            for (int i = 0; i < pageSize; i++)
            {
                int k = offset + i;
                if (k >= searchFilms.Count)
                {
                    break;
                }

                page.Add(searchFilms[k]);
            }

            return page;
        }

        public List<Film> GetPage(int pageNum, int pageSize)
        {
            if (pageNum < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(pageNum));
            }

            if (pageSize < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(pageSize));
            }

            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM films LIMIT $pageSize OFFSET $pageSize * ($pageNumber - 1)";
            command.Parameters.AddWithValue("$pageSize", pageSize);
            command.Parameters.AddWithValue("$pageNumber", pageNum);

            SqliteDataReader reader = command.ExecuteReader();
            List<Film> films = new List<Film>();

            while (reader.Read())
            {
                Film film = new Film();
                film.id = int.Parse(reader.GetString(0));
                film.title = reader.GetString(1);
                film.releaseYear = int.Parse(reader.GetString(2));
                film.country = reader.GetString(3);
                film.director = reader.GetString(4);
                
                films.Add(film);
            }
            reader.Close();
            connection.Close();

            return films;
        }
    }
}
