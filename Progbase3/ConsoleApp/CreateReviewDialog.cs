using Terminal.Gui;

namespace ConsoleApp
{
    public class CreateReviewDialog : Dialog
    {
        public bool canceled;

        protected TextField filmTitleInput;
        protected TextView reviewInput;
        protected RadioGroup ratingRadioGroup;

        public CreateReviewDialog()
        {
            this.Title = "Create review";

            int rightColumnX = 20;
            int inputWidth = 40;

            Label infoLable = new Label(2, 2, "Fill in all of these fields and then enter OK!");
            this.Add(infoLable);

            // film title
            Label filmTitleLbl = new Label(2, 5, "Film title:");
            filmTitleInput = new TextField("")
            {
                X = rightColumnX,
                Y = Pos.Top(filmTitleLbl),
                Width = inputWidth,
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
            };
            reviewField.Add(reviewInput);
            this.Add(reviewLbl, reviewField);

            // rating
            Label ratingLbl = new Label(2, 13, "Rating:");
            ratingRadioGroup = new RadioGroup(rightColumnX, 13, new NStack.ustring[]{"1", "2", "3", "4", "5", "6", "7", "8", "9", "10"})
            {
                DisplayMode = DisplayModeLayout.Horizontal,
                Width = 50,
                Height = 1,
                HorizontalSpace = 3,
            };
            this.Add(ratingLbl, ratingRadioGroup);

            // submit btn
            Button okBtn = new Button("OK")
            {
                X = Pos.Right(filmTitleInput) - 6,
                Y = 15,
            };
            okBtn.Clicked += OnSubmit;
            this.Add(okBtn);

            // cancel btn
            Button cancelBtn = new Button("Cancel")
            {
                X = Pos.Left(okBtn) - 12,
                Y = Pos.Top(okBtn),
            };
            cancelBtn.Clicked += OnCancel;
            this.Add(cancelBtn);
        }

        public virtual Review GetReview()
        {
            Review review = new Review();
            if (filmTitleInput.Text.ToString() == "" || reviewInput.Text.ToString() == "")
            {
                return null;
            }
            review.text = reviewInput.Text.ToString();
            review.rating = ratingRadioGroup.SelectedItem + 1;
            Film film = new Film() { title = filmTitleInput.Text.ToString() };
            review.film = film;
            return review;
        }

        public void OnSubmit()
        {
            this.canceled = false;
            if (this.GetReview() == null)
            {
                MessageBox.ErrorQuery("Error", "All fields must be filled, and release year is integer!", "OK");
                return;
            }
            Application.RequestStop();
        }

        public void OnCancel()
        {
            this.canceled = true;
            Application.RequestStop();
        }
    }
}