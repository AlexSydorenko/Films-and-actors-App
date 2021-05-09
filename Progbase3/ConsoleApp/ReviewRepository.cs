using System;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;

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
                INSERT INTO reviews (text, rating, filmId, userId)
                VALUES ($text, $rating, $filmId, $userId);
            
                SELECT last_insert_rowid();
            ";
            command.Parameters.AddWithValue("$text", review.text);
            command.Parameters.AddWithValue("$rating", review.rating);
            command.Parameters.AddWithValue($"filmId", review.film.id);
            command.Parameters.AddWithValue($"userId", review.author.id);
            
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
            
            Review review = null;
            if (reader.Read())
            {
                review = new Review();
                review.text = reader.GetString(1);
                review.rating = double.Parse(reader.GetString(2));
            }
            reader.Close();
            connection.Close();

            return review;
        }

        public List<Review> GetAllAuthorReviews(int userId)
        {
            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM reviews WHERE userId = $userId";
            command.Parameters.AddWithValue("$userId", userId);
            
            SqliteDataReader reader = command.ExecuteReader();
            
            List<Review> reviews = new List<Review>();
            while (reader.Read())
            {
                Review review = new Review();
                review.text = reader.GetString(1);
                review.rating = double.Parse(reader.GetString(2));
                reviews.Add(review);
            }
            reader.Close();
            connection.Close();
            return reviews;
        }

        public List<Review> GetAllFilmReviews(int filmId)
        {
            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM reviews WHERE filmId = $filmId";
            command.Parameters.AddWithValue("$filmId", filmId);
            
            SqliteDataReader reader = command.ExecuteReader();
            
            List<Review> reviews = new List<Review>();
            while (reader.Read())
            {
                Review review = new Review();
                review.text = reader.GetString(1);
                review.rating = double.Parse(reader.GetString(2));
                reviews.Add(review);
            }
            reader.Close();
            connection.Close();
            return reviews;
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
