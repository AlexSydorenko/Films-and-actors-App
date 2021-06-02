using System;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;

namespace ConsoleApp
{
    public class ActorRepository
    {
        private SqliteConnection connection;
        public ActorRepository(string dbFilepath)
        {
            this.connection = new SqliteConnection($"Data Source = {dbFilepath}");
        }

        public long Insert(Actor actor)
        {
            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = 
            @"
                INSERT INTO actors (fullName, age, residence)
                VALUES ($fullName, $age, $residence);
            
                SELECT last_insert_rowid();
            ";
            command.Parameters.AddWithValue("$fullName", actor.fullName);
            command.Parameters.AddWithValue("$age", actor.age);
            command.Parameters.AddWithValue("$residence", actor.residence);
            
            long newId = (long)command.ExecuteScalar();

            connection.Close();
        
            return newId;
        }

        public Actor GetById(int id)
        {
            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM actors WHERE id = $id";
            command.Parameters.AddWithValue("$id", id);
            
            SqliteDataReader reader = command.ExecuteReader();
            
            Actor actor = null;
            if (reader.Read())
            {
                actor = new Actor();
                actor.id = int.Parse(reader.GetString(0));
                actor.fullName = reader.GetString(1);
                actor.age = int.Parse(reader.GetString(2));
                actor.residence = reader.GetString(3);
            }
            reader.Close();
            connection.Close();

            return actor;
        }

        public bool Delete(int id)
        {
            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"DELETE FROM actors WHERE id = $id";
            command.Parameters.AddWithValue("$id", id);
            
            int nChanged = command.ExecuteNonQuery();
            
            if (nChanged == 0)
            {
                return false;
            }

            connection.Close();

            return true;
        }

        public bool Update(int id, Actor actor)
        {
            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText =
            @"
                UPDATE actors
                    SET fullName = $fullName, age = $age, residence = $residence
                    WHERE id = $id
            ";
            command.Parameters.AddWithValue("$id", id);
            command.Parameters.AddWithValue("$fullName", actor.fullName);
            command.Parameters.AddWithValue("$age", actor.age);
            command.Parameters.AddWithValue("$residence", actor.residence);

            int nChanged = command.ExecuteNonQuery();

            if (nChanged == 0)
            {
                return false;
            }

            connection.Close();
            
            return true;
        }

        public Actor GetByFullName(string actorFullName)
        {
            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM actors WHERE fullName = $fullName";
            command.Parameters.AddWithValue("$fullName", actorFullName);
            
            SqliteDataReader reader = command.ExecuteReader();
            
            Actor actor = null;
            if (reader.Read())
            {
                actor = new Actor();
                actor.id = int.Parse(reader.GetString(0));
                actor.fullName = reader.GetString(1);
                actor.age = int.Parse(reader.GetString(2));
                actor.residence = reader.GetString(3);
            }
            reader.Close();
            connection.Close();

            return actor;
        }

        private List<Actor> SearchActors(string searchValue)
        {
            List<Actor> searchActors = new List<Actor>();
            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM actors
                WHERE fullName LIKE '%' || $searchValue || '%' OR age LIKE '%' || $searchValue || '%'
                OR residence LIKE '%' || $searchValue || '%'";

            command.Parameters.AddWithValue("$searchValue", searchValue);
            SqliteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                Actor actor = new Actor();
                actor.id = int.Parse(reader.GetString(0));
                actor.fullName = reader.GetString(1);
                actor.age = int.Parse(reader.GetString(2));
                actor.residence = reader.GetString(3);
                searchActors.Add(actor);
            }
            connection.Close();

            return searchActors;
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
            return (int)Math.Ceiling(this.SearchActors(searchValue).Count / (double)pageSize);
        }

        public List<Actor> GetSearchPage(string searchValue, int pageNum, int pageSize)
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

            List<Actor> searchActors = this.SearchActors(searchValue);

            List<Actor> page = new List<Actor>();
            int offset = (pageNum - 1) * pageSize;
            for (int i = 0; i < pageSize; i++)
            {
                int k = offset + i;
                if (k >= searchActors.Count)
                {
                    break;
                }

                page.Add(searchActors[k]);
            }

            return page;
        }

        private long GetCount()
        {
            connection.Open();
        
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT COUNT(*) FROM actors";
            
            long count = (long)command.ExecuteScalar();

            connection.Close();
            return count;
        }

        public List<Actor> GetPage(int pageNum, int pageSize)
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
            command.CommandText = @"SELECT * FROM actors LIMIT $pageSize OFFSET $pageSize * ($pageNumber - 1)";
            command.Parameters.AddWithValue("$pageSize", pageSize);
            command.Parameters.AddWithValue("$pageNumber", pageNum);

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
    }
}
