using Terminal.Gui;

namespace ConsoleApp
{
    public class OpenUserDialog : Dialog
    {
        public bool deleted;
        public bool updated;

        private TextField usernameInput;
        private TextField fullNameInput;
        private TextField userRoleInput;
        private User user;
        private User userToEdit;

        public OpenUserDialog(User user)
        {
            this.user = user;
            this.Title = "User info";

            int inputsColumnX = 20;
            int inputWidth = 40;
            this.user = user;

            // Info label
            Label infoLbl = new Label(2, 2, "Full information about user");
            this.Add(infoLbl);

            // Username
            Label titleLbl = new Label(2, 5, "Username:");
            usernameInput = new TextField("")
            {
                X = inputsColumnX, Y = Pos.Top(titleLbl), Width = inputWidth,
                ReadOnly = true
            };
            this.Add(titleLbl, usernameInput);

            // Full name
            Label releaseYearLbl = new Label(2, 7, "Full name:");
            fullNameInput = new TextField("")
            {
                X = inputsColumnX, Y = Pos.Top(releaseYearLbl), Width = inputWidth,
                ReadOnly = true
            };
            this.Add(releaseYearLbl, fullNameInput);

            // Role
            Label countryLbl = new Label(2, 9, "Role:");
            userRoleInput = new TextField("")
            {
                X = inputsColumnX, Y = Pos.Top(countryLbl), Width = inputWidth,
                ReadOnly = true
            };
            this.Add(countryLbl, userRoleInput);

            // Delete button
            Button deleteFilmBtn = new Button("Delete")
            {
                X = inputsColumnX + 30,
                Y = 13
            };
            deleteFilmBtn.Clicked += OnDeleteUser;
            this.Add(deleteFilmBtn);

            // Edit btn
            Button editFilmBtn = new Button("Edit")
            {
                X = Pos.Left(deleteFilmBtn) - 10,
                Y = 13
            };
            editFilmBtn.Clicked += OnEditUser;
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

        public void SetUser(User user)
        {
            // this.user = user;
            this.userToEdit = user;
            this.usernameInput.Text = user.username;
            this.fullNameInput.Text = user.fullName;
            this.userRoleInput.Text = user.role;
        }

        public User GetUser()
        {
            return this.userToEdit;
        }

        public void OnDeleteUser()
        {
            if (this.GetUser().role != "admin")
            {
                MessageBox.ErrorQuery("", "Users can be deleted only by admins!", "OK");
                return;
            }

            int index = MessageBox.Query("Delete user", "Are you sure?", "No", "Yes");
            if (index == 1)
            {
                this.deleted = true;
                Application.RequestStop();
            }
        }

        public void OnEditUser()
        {
            if (this.user.role != "admin")
            {
                MessageBox.ErrorQuery("", "Users can be edited only by admins!", "OK");
                return;
            }

            EditUserDialog dialog = new EditUserDialog(this.user);
            dialog.SetUser(this.userToEdit);

            Application.Run(dialog);

            if (!dialog.canceled)
            {
                User updatedUser = dialog.GetUser();
                this.updated = true;
                this.SetUser(updatedUser);
            }
        }

        public void OnBack()
        {
            Application.RequestStop();
        }
    }
}
