using System;
using Microsoft.Data.Sqlite;

namespace ConsoleApp
{
    public class ActorRepository
    {
        private SqliteConnection connection;
        public ActorRepository(SqliteConnection connection)
        {
            this.connection = connection;
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
    }
}
