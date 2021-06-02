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
        public List<Review> filmReviews = new List<Review>();
        public List<Actor> actors = new List<Actor>();
        
        public override string ToString()
        {
            return string.Format($"{this.title} ({this.releaseYear})");
        }
    }
}
