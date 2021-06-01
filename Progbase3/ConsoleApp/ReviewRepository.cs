using System;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;

namespace ConsoleApp
{
    public class ReviewRepository
    {
        private SqliteConnection connection;
        public ReviewRepository(string dbFilepath)
        {
            this.connection = new SqliteConnection($"Data Source = {dbFilepath}");
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
            command.Parameters.AddWithValue($"filmId", review.filmId);
            command.Parameters.AddWithValue($"userId", review.userId);
            
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
                review.id = int.Parse(reader.GetString(0));
                review.text = reader.GetString(1);
                review.rating = double.Parse(reader.GetString(2));
                review.filmId = int.Parse(reader.GetString(3));
                review.userId = int.Parse(reader.GetString(4));
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

        public bool DeleteAllFilmReviews(int filmId)
        {
            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"DELETE FROM reviews WHERE filmId = $filmId";
            command.Parameters.AddWithValue("$filmId", filmId);
            
            int nChanged = command.ExecuteNonQuery();
            
            if (nChanged == 0)
            {
                return false;
            }

            connection.Close();

            return true;
        }

        public bool DeleteAllAuthorReviews(int userId)
        {
            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"DELETE FROM reviews WHERE userId = $userId";
            command.Parameters.AddWithValue("$userId", userId);
            
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

        public bool AuthorCantAddReview(int userId, int filmId)
        {
            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText =
            @"
                SELECT * FROM reviews
                    WHERE filmId = $filmId AND userId = $userId
            ";
            command.Parameters.AddWithValue("$filmId", filmId);
            command.Parameters.AddWithValue("$userId", userId);

            SqliteDataReader reader = command.ExecuteReader();

            if (reader.Read())
            {
                connection.Close();
                return true;
            }

            connection.Close();
            return false;
        }

        public bool Exists(int userId, int filmId)
        {
            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText =
            @"
                SELECT * FROM reviews
                    WHERE userId = $userId AND filmId = $filmId
            ";
            command.Parameters.AddWithValue("$userId", userId);
            command.Parameters.AddWithValue("$filmId", filmId);

            SqliteDataReader reader = command.ExecuteReader();

            if (reader.Read())
            {
                connection.Close();
                return true;
            }

            connection.Close();
            return false;
        }

        private List<Review> SearchReviews(string searchValue)
        {
            List<Review> searchReviews = new List<Review>();
            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT reviews.id, reviews.text, reviews.rating, reviews.filmId, reviews.userId
                FROM films, reviews, users
	                WHERE reviews.filmId = films.id AND users.id = reviews.userId
		                AND (users.username LIKE '%' || $searchValue || '%'
                        OR films.title LIKE '%' || $searchValue || '%'
                        OR reviews.rating LIKE '%' || $searchValue || '%')";

            command.Parameters.AddWithValue("$searchValue", searchValue);
            SqliteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                Review review = new Review();
                review.id = int.Parse(reader.GetString(0));
                review.text = reader.GetString(1);
                review.rating = int.Parse(reader.GetString(2));
                review.filmId = int.Parse(reader.GetString(3));
                review.userId = int.Parse(reader.GetString(4));

                searchReviews.Add(review);
            }
            connection.Close();

            return searchReviews;
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
            return (int)Math.Ceiling(this.SearchReviews(searchValue).Count / (double)pageSize);
        }

        private long GetCount()
        {
            connection.Open();
        
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT COUNT(*) FROM reviews";
            
            long count = (long)command.ExecuteScalar();

            connection.Close();
            return count;
        }

        public List<Review> GetSearchPage(string searchValue, int pageNum, int pageSize)
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

            List<Review> searchReviews = this.SearchReviews(searchValue);

            List<Review> page = new List<Review>();
            int offset = (pageNum - 1) * pageSize;
            for (int i = 0; i < pageSize; i++)
            {
                int k = offset + i;
                if (k >= searchReviews.Count)
                {
                    break;
                }

                page.Add(searchReviews[k]);
            }

            return page;
        }

        public List<Review> GetPage(int pageNum, int pageSize)
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
            command.CommandText = @"SELECT * FROM reviews LIMIT $pageSize OFFSET $pageSize * ($pageNumber - 1)";
            command.Parameters.AddWithValue("$pageSize", pageSize);
            command.Parameters.AddWithValue("$pageNumber", pageNum);

            SqliteDataReader reader = command.ExecuteReader();
            List<Review> reviews = new List<Review>();

            while (reader.Read())
            {
                Review review = new Review();
                review.id = int.Parse(reader.GetString(0));
                review.text = reader.GetString(1);
                review.rating = int.Parse(reader.GetString(2));
                review.filmId = int.Parse(reader.GetString(3));
                review.userId = int.Parse(reader.GetString(4));
                
                reviews.Add(review);
            }
            reader.Close();
            connection.Close();

            return reviews;
        }
    }
}
