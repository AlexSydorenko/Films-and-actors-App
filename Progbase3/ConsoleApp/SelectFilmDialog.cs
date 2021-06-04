using Terminal.Gui;

namespace ConsoleApp
{
    public class SelectFilmDialog : Dialog
    {
        public bool canceled;
        private string databasePath;
        private TextField filmTitleInput;

        public SelectFilmDialog(string databasePath)
        {
            this.Title = "Select film";
            this.databasePath = databasePath;

            Label filmTitleLbl = new Label("Film title:")
            {
                X = 2,
                Y = Pos.Center(),
            };
            filmTitleInput = new TextField("")
            {
                X = Pos.Right(filmTitleLbl) + 1,
                Y = Pos.Top(filmTitleLbl),
                Width = 30,
            };
            this.Add(filmTitleLbl, filmTitleInput);

            Button cancelBtn = new Button("Cancel")
            {
                X = Pos.Right(filmTitleInput) - 18,
                Y = Pos.Bottom(filmTitleInput) + 2,
            };
            cancelBtn.Clicked += OnCancel;
            Button okBtn = new Button("OK")
            {
                X = Pos.Right(cancelBtn) + 2,
                Y = Pos.Top(cancelBtn),
            };
            okBtn.Clicked += OnSubmit;
            this.Add(cancelBtn, okBtn);
        }

        public string GetFilmTitle()
        {
            return this.filmTitleInput.Text.ToString();
        }

        public void OnSubmit()
        {
            this.canceled = false;
            Film film = new FilmRepository(databasePath).GetByTitle(this.GetFilmTitle());
            if (film == null)
            {
                MessageBox.ErrorQuery("Error", "No such film in the database!", "OK");
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
