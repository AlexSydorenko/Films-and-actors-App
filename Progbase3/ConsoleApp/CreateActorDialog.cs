using Terminal.Gui;

namespace ConsoleApp
{
    public class CreateActorDialog : Dialog
    {
        public bool canceled;

        protected TextField fullnameInput;
        protected TextField ageInput;
        protected TextField residenceInput;
        protected TextField filmInWhichStarredInput;
        protected Label filmInWhichStarredLbl;

        public CreateActorDialog()
        {
            this.Title = "Create actor";

            int rightColumnX = 25;
            int inputWidth = 40;

            Label infoLable = new Label(2, 2, "Fill in all of these fields and then enter OK!");
            this.Add(infoLable);

            // full name
            Label fullnameLbl = new Label(2, 5, "Full name:");
            fullnameInput = new TextField("")
            {
                X = rightColumnX,
                Y = Pos.Top(fullnameLbl),
                Width = inputWidth,
            };
            this.Add(fullnameLbl, fullnameInput);

            // age
            Label ageLbl = new Label(2, 7, "Actor's age:");
            ageInput = new TextField("")
            {
                X = rightColumnX,
                Y = Pos.Top(ageLbl),
                Width = inputWidth,
            };
            this.Add(ageLbl, ageInput);

            // residence
            Label residenceLbl = new Label(2, 9, "Residence:");
            residenceInput = new TextField("")
            {
                X = rightColumnX,
                Y = Pos.Top(residenceLbl),
                Width = inputWidth,
            };
            this.Add(residenceLbl, residenceInput);

            // film in which starred
            filmInWhichStarredLbl = new Label(2, 11, "Film in which starred:");
            filmInWhichStarredInput = new TextField("")
            {
                X = rightColumnX,
                Y = Pos.Top(filmInWhichStarredLbl),
                Width = inputWidth,
            };
            this.Add(filmInWhichStarredLbl, filmInWhichStarredInput);

            // submit btn
            Button okBtn = new Button("OK")
            {
                X = Pos.Right(filmInWhichStarredInput) - 6,
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

        public virtual Actor GetActor()
        {
            Actor actor = new Actor();
            if (fullnameInput.Text == "" || !int.TryParse(ageInput.Text.ToString(), out actor.age) ||
                residenceInput.Text == "" || filmInWhichStarredInput.Text == "")
            {
                return null;
            }
            actor.fullName = fullnameInput.Text.ToString();
            actor.residence = residenceInput.Text.ToString();
            Film film = new Film() { title = filmInWhichStarredInput.Text.ToString() };
            actor.films.Add(film);
            return actor;
        }

        public void OnSubmit()
        {
            this.canceled = false;
            if (this.GetActor() == null)
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
