using System;
using System.Collections.Generic;

namespace ConsoleApp
{
    public class Actor
    {
        public int id;
        public string fullName;
        public int age;
        public string residence;
        public List<Film> films;

        public override string ToString()
        {
            return string.Format($"{this.fullName}, {this.age} years old, from {this.residence}");
        }
    }
}
