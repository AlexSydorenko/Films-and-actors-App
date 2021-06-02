using Terminal.Gui;

namespace ConsoleApp
{
    public class OpenFilmDialog : Dialog
    {
        public bool deleted;
        public bool updated;

        private TextField titleInput;
        private TextField releaseYearInput;
        private TextField countryInput;
        private TextField directorInput;
        protected Film film;
        private User user;

        public OpenFilmDialog(User user)
        {
            this.Title = "Film info";

            int inputsColumnX = 20;
            int inputWidth = 40;
            this.user = user;

            // Info label
            Label infoLbl = new Label(2, 2, "Full information about film");
            this.Add(infoLbl);

            // Title
            Label titleLbl = new Label(2, 5, "Film title:");
            titleInput = new TextField("")
            {
                X = inputsColumnX, Y = Pos.Top(titleLbl), Width = inputWidth,
                ReadOnly = true
            };
            this.Add(titleLbl, titleInput);

            // Release year
            Label releaseYearLbl = new Label(2, 7, "Release year:");
            releaseYearInput = new TextField("")
            {
                X = inputsColumnX, Y = Pos.Top(releaseYearLbl), Width = inputWidth,
                ReadOnly = true
            };
            this.Add(releaseYearLbl, releaseYearInput);

            // Country
            Label countryLbl = new Label(2, 9, "Country:");
            countryInput = new TextField("")
            {
                X = inputsColumnX, Y = Pos.Top(countryLbl), Width = inputWidth,
                ReadOnly = true
            };
            this.Add(countryLbl, countryInput);

            // Director
            Label directorLbl = new Label(2, 11, "Director:");
            directorInput = new TextField("")
            {
                X = inputsColumnX, Y = Pos.Top(directorLbl), Width = inputWidth,
            };
            this.Add(directorLbl, directorInput);

            // Delete button
            Button deleteFilmBtn = new Button("Delete")
            {
                X = inputsColumnX + 30,
                Y = 13
            };
            deleteFilmBtn.Clicked += OnDeleteFilm;
            this.Add(deleteFilmBtn);

            // Edit button
            Button editFilmBtn = new Button("Edit")
            {
                X = Pos.Left(deleteFilmBtn) - 10,
                Y = 13
            };
            editFilmBtn.Clicked += OnEditFilm;
            this.Add(editFilmBtn);

            // Back btn
            Button backBtn = new Button("Back")
            {
                X = 2,
                Y = 13,
            };
            backBtn.Clicked += OnBack;
            this.Add(backBtn);
        }

        public void SetFilm(Film film)
        {
            this.film = film;
            this.titleInput.Text = film.title;
            this.releaseYearInput.Text = film.releaseYear.ToString();
            this.countryInput.Text = film.country;
            this.directorInput.Text = film.director;
        }

        public Film GetFilm()
        {
            return this.film;
        }

        public void OnDeleteFilm()
        {
            if (this.user.role != "admin")
            {
                MessageBox.ErrorQuery("", "Films can be deleted only by admins!", "OK");
                return;
            }

            int index = MessageBox.Query("Delete film", "Are you sure?", "No", "Yes");
            if (index == 1)
            {
                this.deleted = true;
                Application.RequestStop();
            }
        }

        public void OnEditFilm()
        {
            if (this.user.role != "admin")
            {
                MessageBox.ErrorQuery("", "Films can be edited only by admins!", "OK");
                return;
            }

            EditFilmDialog dialog = new EditFilmDialog();
            dialog.SetFilm(this.film);

            Application.Run(dialog);

            if (!dialog.canceled)
            {
                Film updatedFilm = dialog.GetFilm();
                if (updatedFilm == null)
                {
                    MessageBox.ErrorQuery("Error", "All text fields must be filled, and release year is integer!", "Try again");
                    return;
                }
                this.updated = true;
                this.SetFilm(updatedFilm);
            }
        }

        public void OnBack()
        {
            Application.RequestStop();
        }
    }
}
