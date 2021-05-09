using System;

namespace ConsoleApp
{
    public class Review
    {
        public int id;
        public string text;
        public double rating;
        public Film film;
        public User author;

        public override string ToString()
        {
            return string.Format($"Review: {this.text}\n" +
                $"Rating: {this.rating}");
        }
    }
}
