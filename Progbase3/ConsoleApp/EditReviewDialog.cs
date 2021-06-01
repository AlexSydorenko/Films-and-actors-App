using Terminal.Gui;

namespace ConsoleApp
{
    public class EditReviewDialog : CreateReviewDialog
    {
        public EditReviewDialog()
        {
            this.Title = "Edit review";

            this.filmTitleInput.ReadOnly = true;
        }

        public void SetReview(Review review)
        {
            this.filmTitleInput.Text = review.film.title;
            this.reviewInput.Text = review.text;
            this.ratingRadioGroup.SelectedItem = (int)review.rating - 1;
        }

        // public override Review GetReview()
        // {
        //     Review review = new Review();
        //     if (reviewInput.Text.ToString() == "")
        //     {
        //         return null;
        //     }
        //     review.text = reviewInput.Text.ToString();
        //     review.rating = ratingRadioGroup.SelectedItem + 1;
        //     Film film = new Film() { title = filmTitleInput.Text.ToString() };
        //     review.film = film;
            
        //     return review;
        // }
    }
}