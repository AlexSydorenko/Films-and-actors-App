using System;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;

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
                user.role = reader.GetString(4);
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
                user.role = reader.GetString(4);
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

        public bool Update(int id, User user)
        {
            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText =
            @"
                UPDATE users
                    SET username = $username, fullName = $fullName, role = $role
                    WHERE id = $id
            ";
            command.Parameters.AddWithValue("$id", id);
            command.Parameters.AddWithValue("$username", user.username);
            command.Parameters.AddWithValue("$fullName", user.fullName);
            command.Parameters.AddWithValue("$role", user.role);

            int nChanged = command.ExecuteNonQuery();

            if (nChanged == 0)
            {
                return false;
            }

            connection.Close();
            
            return true;
        }

        private List<User> SearchUsers(string searchValue)
        {
            List<User> searchUsers = new List<User>();
            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM users
                WHERE id LIKE '%' || $searchValue || '%' OR username LIKE '%' || $searchValue || '%'
                OR fullName LIKE '%' || $searchValue || '%' OR role LIKE '%' || $searchValue || '%'";

            command.Parameters.AddWithValue("$searchValue", searchValue);
            SqliteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                User user = new User();
                user.id = int.Parse(reader.GetString(0));
                user.username = reader.GetString(1);
                user.fullName = reader.GetString(3);
                user.role = reader.GetString(4);
                searchUsers.Add(user);
            }
            connection.Close();

            return searchUsers;
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
            return (int)Math.Ceiling(this.SearchUsers(searchValue).Count / (double)pageSize);
        }

        private long GetCount()
        {
            connection.Open();
        
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT COUNT(*) FROM users";
            
            long count = (long)command.ExecuteScalar();

            connection.Close();
            return count;
        }

        public List<User> GetSearchPage(string searchValue, int pageNum, int pageSize)
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

            List<User> searchFilms = this.SearchUsers(searchValue);

            List<User> page = new List<User>();
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

        public List<User> GetPage(int pageNum, int pageSize)
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
            command.CommandText = @"SELECT * FROM users LIMIT $pageSize OFFSET $pageSize * ($pageNumber - 1)";
            command.Parameters.AddWithValue("$pageSize", pageSize);
            command.Parameters.AddWithValue("$pageNumber", pageNum);

            SqliteDataReader reader = command.ExecuteReader();
            List<User> users = new List<User>();

            while (reader.Read())
            {
                User user = new User();
                user.id = int.Parse(reader.GetString(0));
                user.username = reader.GetString(1);
                user.fullName = reader.GetString(3);
                user.role = reader.GetString(4);
                
                users.Add(user);
            }
            reader.Close();
            connection.Close();

            return users;
        }
    }
}
