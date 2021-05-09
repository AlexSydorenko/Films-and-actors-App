using System;
using System.Collections.Generic;

namespace ConsoleApp
{
    public class Film
    {
        public int id;
        public string title;
        public int releaseYear;
        public string country;
        public string director;
        public List<Review> filmReviews;
        public List<Actor> actors;

        public override string ToString()
        {
            return string.Format($"Title: {this.title}\n" +
                $"Release year: {this.releaseYear}\n" +
                $"Country: {this.country}\n" +
                $"Director: {this.director}");
        }
    }
}
