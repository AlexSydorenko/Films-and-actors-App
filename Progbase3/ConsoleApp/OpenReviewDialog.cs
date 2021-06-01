using Terminal.Gui;

namespace ConsoleApp
{
    public class OpenReviewDialog : Dialog
    {
        public bool deleted;
        public bool updated;

        private TextField filmTitleInput;
        private TextView reviewInput;
        private Label ratingLabel;
        private TextField authorInput;
        protected Review review;

        public OpenReviewDialog()
        {
            this.Title = "Review info";

            int rightColumnX = 20;
            int inputWidth = 40;

            Label infoLable = new Label(2, 2, "Full information about review");
            this.Add(infoLable);

            // film title
            Label filmTitleLbl = new Label(2, 5, "Film title:");
            filmTitleInput = new TextField("")
            {
                X = rightColumnX,
                Y = Pos.Top(filmTitleLbl),
                Width = inputWidth,
                ReadOnly = true,
            };
            this.Add(filmTitleLbl, filmTitleInput);

            // review text
            FrameView reviewField = new FrameView()
            {
                X = rightColumnX,
                Y = 7,
                Width = inputWidth,
                Height = 5,
            };
            Label reviewLbl = new Label(2, 8, "Your review:");
            reviewInput = new TextView()
            {
                // X = rightColumnX,
                // Y = Pos.Top(reviewLbl),
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                ReadOnly = true,
            };
            reviewField.Add(reviewInput);
            this.Add(reviewLbl, reviewField);

            // rating
            Label ratingLbl = new Label(2, 13, "Rating:");
            ratingLabel = new Label(".")
            {
                // DisplayMode = DisplayModeLayout.Horizontal,
                X = rightColumnX,
                Y = Pos.Top(ratingLbl),
                Width = inputWidth,
                // HorizontalSpace = 3,
                
            };
            this.Add(ratingLbl, ratingLabel);

            // username
            Label authorLbl = new Label(2, 15, "Author's username:");
            authorInput = new TextField("")
            {
                X = rightColumnX,
                Y = Pos.Top(authorLbl),
                Width = inputWidth,
                ReadOnly = true,
            };
            this.Add(authorLbl, authorInput);

            // Delete button
            Button deleteReviewBtn = new Button("Delete")
            {
                X = rightColumnX + 30,
                Y = Pos.Bottom(authorInput) + 2,
            };
            deleteReviewBtn.Clicked += OnDeleteReview;
            this.Add(deleteReviewBtn);

            // Edit button
            Button editReviewBtn = new Button("Edit")
            {
                X = Pos.Left(deleteReviewBtn) - 10,
                Y = Pos.Top(deleteReviewBtn),
            };
            editReviewBtn.Clicked += OnEditReview;
            this.Add(editReviewBtn);

            // Back btn
            Button backBtn = new Button("Back")
            {
                X = 2,
                Y = Pos.Top(deleteReviewBtn),
            };
            backBtn.Clicked += OnBack;
            this.Add(backBtn);
        }

        public Review GetReview()
        {
            return this.review;
        }

        public void SetReview(Review review)
        {
            this.review = review;
            this.filmTitleInput.Text = review.film.title;
            this.reviewInput.Text = review.text;
            this.ratingLabel.Text = review.rating.ToString();
            this.authorInput.Text = review.author.username;
        }

        public void OnDeleteReview()
        {
            int index = MessageBox.Query("Delete review", "Are you sure?", "No", "Yes");
            if (index == 1)
            {
                this.deleted = true;
                Application.RequestStop();
            }
        }

        public void OnEditReview()
        {
            EditReviewDialog dialog = new EditReviewDialog();
            dialog.SetReview(this.review);

            Application.Run(dialog);

            if (!dialog.canceled)
            {
                Review updatedReview = dialog.GetReview(); // ERROR HERE
                updatedReview.author = this.review.author;
                if (updatedReview == null)
                {
                    MessageBox.ErrorQuery("Error", "All text fields must be filled!", "Try again");
                    return;
                }
                this.updated = true;
                this.SetReview(updatedReview);
            }
        }

        public void OnBack()
        {
            Application.RequestStop();
        }
    }
}