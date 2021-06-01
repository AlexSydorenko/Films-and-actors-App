using System;
using Microsoft.Data.Sqlite;

namespace ConsoleApp
{
    public class UserRepository
    {
        private SqliteConnection connection;

        public UserRepository(string dbFilepath)
        {
            this.connection = new SqliteConnection($"Data Source = {dbFilepath}");
        }

        public long Insert(User user)
        {
            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = 
            @"
                INSERT INTO users (username, password, fullName)
                VALUES ($username, $password, $fullName);
            
                SELECT last_insert_rowid();
            ";
            command.Parameters.AddWithValue("$username", user.username);
            command.Parameters.AddWithValue("$password", user.password);
            command.Parameters.AddWithValue("$fullName", user.fullName);
            
            long newId = (long)command.ExecuteScalar();

            connection.Close();
        
            return newId;
        }

        public User GetByUsername(string username)
        {
            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM users WHERE username = $username";
            command.Parameters.AddWithValue("$username", username);
            
            SqliteDataReader reader = command.ExecuteReader();
            
            User user = null;
            if (reader.Read())
            {
                user = new User();
                user.id = int.Parse(reader.GetString(0));
                user.username = reader.GetString(1);
                user.password = reader.GetString(2);
                user.fullName = reader.GetString(3);
            }
            reader.Close();
            connection.Close();

            return user;
        }

        public User GetById(int id)
        {
            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM users WHERE id = $id";
            command.Parameters.AddWithValue("$id", id);
            
            SqliteDataReader reader = command.ExecuteReader();
            
            User user = null;
            if (reader.Read())
            {
                user = new User();
                user.id = int.Parse(reader.GetString(0));
                user.username = reader.GetString(1);
                user.password = reader.GetString(2);
                user.fullName = reader.GetString(3);
            }
            reader.Close();
            connection.Close();

            return user;
        }

        public bool Delete(string username)
        {
            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"DELETE FROM users WHERE username = $username";
            command.Parameters.AddWithValue("$username", username);
            
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
