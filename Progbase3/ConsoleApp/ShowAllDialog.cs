using Terminal.Gui;

namespace ConsoleApp
{
    public class ShowAllDialog : Dialog
    {
        private string databasePath;
        private User user;

        private RadioGroup radioGroup;

        public ShowAllDialog(string databasePath, User user)
        {
            this.databasePath = databasePath;
            this.user = user;

            this.Title = "Select";

            Label chooseVariantLbl = new Label(2, 5, "Select which list you want to watch:");
            this.Add(chooseVariantLbl);

            radioGroup = new RadioGroup(2, 7, new NStack.ustring[]{"Films", "Actors", "Reviews", "Users"});
            this.Add(radioGroup);

            Button backBtn = new Button(2, 12, "Back");
            backBtn.Clicked += OnBack;
            this.Add(backBtn);
            
            Button okBtn = new Button("OK")
            {
                X = Pos.Right(backBtn) + 2,
                Y = Pos.Top(backBtn),
            };
            okBtn.Clicked += OnSubmit;
            this.Add(okBtn);
        }

        public void OnSubmit()
        {
            int selected = radioGroup.SelectedItem;
            if (selected == 0)
            {
                FilmsListDialog dialog = new FilmsListDialog(this.user);
                dialog.SetRepository(new FilmRepository(databasePath), new FilmActorsRepository(databasePath), new ReviewRepository(databasePath));
                Application.Run(dialog);
            }
            else if (selected == 1)
            {
                ActorsListDialog dialog = new ActorsListDialog(this.user);
                dialog.SetRepository(new ActorRepository(databasePath), new FilmActorsRepository(databasePath));
                Application.Run(dialog);
            }
            else if (selected == 2)
            {
                ReviewsListDialog dialog = new ReviewsListDialog(this.user);
                dialog.SetRepository(new ReviewRepository(databasePath), new FilmRepository(databasePath), new UserRepository(databasePath));
                Application.Run(dialog);
            }
            else if (selected == 3)
            {
                UsersListDialog dialog = new UsersListDialog(this.user);
                dialog.SetRepository(new UserRepository(databasePath), new ReviewRepository(databasePath));
                Application.Run(dialog);
            }
        }
        
        public void OnBack()
        {
            Application.RequestStop();
        }
    }
}
