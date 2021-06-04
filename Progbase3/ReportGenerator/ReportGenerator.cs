using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Collections.Generic;
using System.IO.Compression;

namespace ConsoleApp
{
    public class ReportGenerator
    {
        private string databasePath;
        public ReportGenerator(string databasePath)
        {
            this.databasePath = databasePath;
        }

        public void GenerateReport(Film film)
        {
            XElement root = XElement.Load("/home/alex/projects/progbase3/Progbase3/report/content.xml");
            string[] filmData = GetAllFilmInformation(film);
            FindAndReplace(root, filmData);
            root.Save("/home/alex/projects/progbase3/Progbase3/report/content.xml");
            ZipFile.CreateFromDirectory("../report", $"../{filmData[0]}.docx");
        }

        public string[] GetAllFilmInformation(Film film)
        {
            string[] filmData = new string[7];
            filmData[0] = film.title;
            filmData[1] = film.releaseYear.ToString();
            List<Actor> allFilmActors = new FilmActorsRepository(databasePath).GetActorsFromTheFilm(film.id);
            filmData[2] = allFilmActors.Count.ToString();
            List<Review> allFilmReviews = new ReviewRepository(databasePath).GetAllFilmReviews(film.id);
            filmData[3] = allFilmReviews.Count.ToString();
            filmData[4] = GetAvarageFilmRating(allFilmReviews).ToString();
            Review maxRatingReview = GetMaxRatingReview(allFilmReviews);
            filmData[5] = $"[{maxRatingReview.id}] {maxRatingReview.text} ({maxRatingReview.rating})";
            Review minRatingReview = GetMinRatingReview(allFilmReviews);
            filmData[6] = $"[{minRatingReview.id}] {minRatingReview.text} ({minRatingReview.rating})";

            return filmData;
        }

        public double GetAvarageFilmRating(List<Review> reviews)
        {
            double sum = 0;
            foreach (Review review in reviews)
            {
                sum += review.rating;
            }
            return sum / reviews.Count;
        }

        public Review GetMaxRatingReview(List<Review> reviews)
        {
            Review maxRatingReview = reviews[0];
            foreach (Review review in reviews)
            {
                if (review.rating > maxRatingReview.rating)
                {
                    maxRatingReview = review;
                }
            }
            return maxRatingReview;
        }

        public Review GetMinRatingReview(List<Review> reviews)
        {
            Review minRatingReview = reviews[0];
            foreach (Review review in reviews)
            {
                if (review.rating < minRatingReview.rating)
                {
                    minRatingReview = review;
                }
            }
            return minRatingReview;
        }

        static void FindAndReplace(XElement node, string[] filmData)
        {
            if (node.FirstNode != null
                && node.FirstNode.NodeType == XmlNodeType.Text)
            {
                switch (node.Value)
                {
                    case "{title}": node.Value = filmData[0]; break;
                    case "{releaseYear}": node.Value = filmData[1]; break;
                    case "{numOfActors}": node.Value = filmData[2]; break;
                    case "{numOfReviews}": node.Value = filmData[3]; break;
                    case "{rating}": node.Value = filmData[4]; break;
                    case "{maxRatingReview}": node.Value = filmData[5]; break;
                    case "{minRatingReview}": node.Value = filmData[6]; break;
                }
            }
        
            foreach (XElement el in node.Elements())
            {
                FindAndReplace(el, filmData);
            }
        }

    }
}
