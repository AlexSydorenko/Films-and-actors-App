using Terminal.Gui;

namespace ConsoleApp
{
    public class OpenActorDialog : Dialog
    {
        public bool deleted;
        public bool updated;

        private TextField fullnameInput;
        private TextField ageInput;
        private TextField residenceInput;
        protected Actor actor;

        public OpenActorDialog()
        {
            this.Title = "Actor info";

            int inputsColumnX = 20;
            int inputWidth = 40;

            // Info label
            Label infoLbl = new Label(2, 2, "Full information about actor");
            this.Add(infoLbl);

            // Full name
            Label titleLbl = new Label(2, 5, "Full name:");
            fullnameInput = new TextField("")
            {
                X = inputsColumnX, Y = Pos.Top(titleLbl), Width = inputWidth,
                ReadOnly = true
            };
            this.Add(titleLbl, fullnameInput);

            // Age
            Label releaseYearLbl = new Label(2, 7, "Age:");
            ageInput = new TextField("")
            {
                X = inputsColumnX, Y = Pos.Top(releaseYearLbl), Width = inputWidth,
                ReadOnly = true
            };
            this.Add(releaseYearLbl, ageInput);

            // Residence
            Label countryLbl = new Label(2, 9, "Residence:");
            residenceInput = new TextField("")
            {
                X = inputsColumnX, Y = Pos.Top(countryLbl), Width = inputWidth,
                ReadOnly = true
            };
            this.Add(countryLbl, residenceInput);

            // Delete button
            Button deleteFilmBtn = new Button("Delete")
            {
                X = inputsColumnX + 30,
                Y = 11
            };
            deleteFilmBtn.Clicked += OnDeleteActor;
            this.Add(deleteFilmBtn);

            // Edit button
            Button editFilmBtn = new Button("Edit")
            {
                X = Pos.Left(deleteFilmBtn) - 10,
                Y = 11
            };
            editFilmBtn.Clicked += OnEditActor;
            this.Add(editFilmBtn);

            // Back btn
            Button backBtn = new Button("Back")
            {
                X = 2,
                Y = 11,
            };
            backBtn.Clicked += OnBack;
            this.Add(backBtn);
        }

        public void SetActor(Actor actor)
        {
            this.actor = actor;
            this.fullnameInput.Text = actor.fullName;
            this.ageInput.Text = actor.age.ToString();
            this.residenceInput.Text = actor.residence;
        }

        public Actor GetActor()
        {
            return this.actor;
        }

        public void OnDeleteActor()
        {
            int index = MessageBox.Query("Delete actor", "Are you sure?", "No", "Yes");
            if (index == 1)
            {
                this.deleted = true;
                Application.RequestStop();
            }
        }

        public void OnEditActor()
        {
            EditActorDialog dialog = new EditActorDialog();
            dialog.SetActor(this.actor);

            Application.Run(dialog);

            if (!dialog.canceled)
            {
                Actor updatedActor = dialog.GetActor();
                if (updatedActor == null)
                {
                    MessageBox.ErrorQuery("Error", "All text fields must be filled, and actor's age is integer!", "Try again");
                    return;
                }
                this.updated = true;
                this.SetActor(updatedActor);
            }
        }

        public void OnBack()
        {
            Application.RequestStop();
        }
    }
}