using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.IO;

namespace ConsoleApp
{
    public class DataExportAndImport
    {
        public static void ExportFilmReviews(ReviewRepository reviewRepo, int filmId, string filePath)
        {
            List<Review> filmReviews = reviewRepo.GetAllFilmReviews(filmId);
            XmlSerializer ser = new XmlSerializer(typeof(List<Review>));
            System.IO.StreamWriter writer = new System.IO.StreamWriter(filePath);
            ser.Serialize(writer, filmReviews);
            writer.Close();
        }

        public static List<Review> ImportFilmReviews(string filePath)
        {
            XmlSerializer ser = new XmlSerializer(typeof(List<Review>));
            StreamReader reader = new StreamReader(filePath);
            List<Review> allReviews = (List<Review>)ser.Deserialize(reader);
            reader.Close();

            int filmId = allReviews[0].filmId;

            foreach (Review review in allReviews)
            {
                if (review.filmId != filmId)
                {
                    throw new ArgumentOutOfRangeException(nameof(review));
                }
            }

            return allReviews;
        }
    }
}
