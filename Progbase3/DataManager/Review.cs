using System;

namespace ConsoleApp
{
    public class Review
    {
        public int id;
        public string text;
        public double rating;
        public int filmId;
        public int userId;

        public Film film;
        public User author;

        public override string ToString()
        {
            return string.Format($"[{author.username}] {this.film.title} ({this.rating})");
        }
    }
}
