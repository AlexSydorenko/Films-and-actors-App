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
    }
}