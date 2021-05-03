using System;
using Microsoft.Data.Sqlite;

namespace ConsoleApp
{
    public class ReviewRepository
    {
        private SqliteConnection connection;
        public ReviewRepository(SqliteConnection connection)
        {
            this.connection = connection;
        }

        public long Insert(Review review)
        {
            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = 
            @"
                INSERT INTO reviews (text, rating)
                VALUES ($text, $rating);
            
                SELECT last_insert_rowid();
            ";
            command.Parameters.AddWithValue("$text", review.text);
            command.Parameters.AddWithValue("$rating", review.rating);
            
            long newId = (long)command.ExecuteScalar();

            connection.Close();
        
            return newId;
        }

        public Review GetById(int id)
        {
            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM reviews WHERE id = $id";
            command.Parameters.AddWithValue("$id", id);
            
            SqliteDataReader reader = command.ExecuteReader();
            
            if (reader.Read())
            {
                Review review = new Review();
                review.text = reader.GetString(1);
                review.rating = double.Parse(reader.GetString(2));

                return review;
            }
            reader.Close();
            connection.Close();

            return null;
        }

        public bool Delete(int id)
        {
            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"DELETE FROM reviews WHERE id = $id";
            command.Parameters.AddWithValue("$id", id);
            
            int nChanged = command.ExecuteNonQuery();
            
            if (nChanged == 0)
            {
                return false;
            }

            connection.Close();

            return true;
        }

        public bool Update(int id, Review review)
        {
            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText =
            @"
                UPDATE reviews
                    SET text = $text, rating = $rating
                    WHERE id = $id
            ";
            command.Parameters.AddWithValue("$id", id);
            command.Parameters.AddWithValue("$text", review.text);
            command.Parameters.AddWithValue("$rating", review.rating);

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
