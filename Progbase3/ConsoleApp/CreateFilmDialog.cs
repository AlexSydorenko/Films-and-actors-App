using Terminal.Gui;

namespace ConsoleApp
{
    public class CreateFilmDialog : Dialog
    {
        public bool canceled;

        protected TextField titleInput;
        protected TextField releaseYearInput;
        protected TextField countryInput;
        protected TextField directorInput;

        public CreateFilmDialog()
        {
            this.Title = "Create film";

            int rightColumnX = 20;
            int inputWidth = 40;

            Label infoLable = new Label(2, 2, "Fill in all of these fields and then enter OK!");
            this.Add(infoLable);

            // title
            Label titleLbl = new Label(2, 5, "Film title:");
            titleInput = new TextField("")
            {
                X = rightColumnX,
                Y = Pos.Top(titleLbl),
                Width = inputWidth,
            };
            this.Add(titleLbl, titleInput);

            // release year
            Label releaseYearLbl = new Label(2, 7, "Release year:");
            releaseYearInput = new TextField("")
            {
                X = rightColumnX,
                Y = Pos.Top(releaseYearLbl),
                Width = inputWidth,
            };
            this.Add(releaseYearLbl, releaseYearInput);

            // country
            Label countryLbl = new Label(2, 9, "Country:");
            countryInput = new TextField("")
            {
                X = rightColumnX,
                Y = Pos.Top(countryLbl),
                Width = inputWidth,
            };
            this.Add(countryLbl, countryInput);

            // director
            Label directorLbl = new Label(2, 11, "Director:");
            directorInput = new TextField("")
            {
                X = rightColumnX,
                Y = Pos.Top(directorLbl),
                Width = inputWidth,
            };
            this.Add(directorLbl, directorInput);

            // submit btn
            Button okBtn = new Button("OK")
            {
                X = Pos.Right(directorInput) - 6,
                Y = 13,
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

        public Film GetFilm()
        {
            Film film = new Film();
            if (titleInput.Text == "" || !int.TryParse(releaseYearInput.Text.ToString(), out film.releaseYear) ||
                countryInput.Text == "" || directorInput.Text == "")
            {
                return null;
            }
            film.title = titleInput.Text.ToString();
            film.country = countryInput.Text.ToString();
            film.director = directorInput.Text.ToString();
            return film;
        }

        public void OnSubmit()
        {
            this.canceled = false;
            if (this.GetFilm() == null)
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
