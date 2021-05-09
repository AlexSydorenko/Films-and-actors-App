using System;
using System.Collections.Generic;

namespace ConsoleApp
{
    public class User
    {
        public int id;
        public string username;
        public string password;
        public string fullName;
        public List<Review> userReviews;

        public override string ToString()
        {
            return string.Format($"({id}) {username} | {fullName}");
        }
    }
}
