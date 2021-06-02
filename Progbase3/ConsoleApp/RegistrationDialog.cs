using Terminal.Gui;

namespace ConsoleApp
{
    public class RegistrationDialog : Dialog
    {
        public bool canceled;
        private string databasePath;

        private TextField usernameInput;
        private TextField passwordInput;
        private TextField fullnameInput;

        public RegistrationDialog(string databasePath)
        {
            this.Title = "Sign Up";
            int inputWidth = 40;
            this.databasePath = databasePath;

            Label infoLbl = new Label("Please, fill in all of these fields")
            {
                X = Pos.Center(),
                Y = 2,
            };
            this.Add(infoLbl);

            // username
            Label usernameLbl = new Label("Username:")
            {
                X = Pos.Center() - 20,
                Y = 4,
            };
            usernameInput = new TextField("")
            {
                X = Pos.Left(usernameLbl),
                Y = 5,
                Width = inputWidth,
            };
            this.Add(usernameLbl, usernameInput);

            // password
            Label passwordLbl = new Label("Password (at less 6 characters):")
            {
                X = Pos.Left(usernameLbl),
                Y = 7,
            };
            passwordInput = new TextField("")
            {
                X = Pos.Left(passwordLbl),
                Y = 8,
                Width = inputWidth,
                Secret = true,
            };
            this.Add(passwordLbl, passwordInput);

            // fullname
            Label fullnameLbl = new Label("Full name:")
            {
                X = Pos.Left(passwordLbl),
                Y = 10,
            };
            fullnameInput = new TextField("")
            {
                X = Pos.Left(fullnameLbl),
                Y = 11,
                Width = inputWidth,
            };
            this.Add(fullnameLbl, fullnameInput);

            // cancel btn
            Button cancelBtn = new Button("Cancel")
            {
                X = Pos.Center() - 10,
                Y = 13,
            };
            cancelBtn.Clicked += OnCancel;
            this.Add(cancelBtn);

            // submit btn
            Button okBtn = new Button("OK")
            {
                X = Pos.Right(cancelBtn) + 2,
                Y = Pos.Top(cancelBtn),
            };
            okBtn.Clicked += OnSubmit;
            this.Add(okBtn);
        }

        public User GetUser()
        {
            User user = new User();
            if (this.usernameInput.Text == "" || this.passwordInput.Text == "" || this.fullnameInput.Text == "")
            {
                return null;
            }
            user.username = this.usernameInput.Text.ToString();
            user.password = this.passwordInput.Text.ToString();
            user.fullName = this.fullnameInput.Text.ToString();
            return user;
        }

        public void OnCancel()
        {
            this.canceled = true;
            Application.RequestStop();
        }

        public void OnSubmit()
        {
            this.canceled = false;
            if (this.GetUser() == null)
            {
                MessageBox.ErrorQuery("Error", "All fields must be filled!", "OK");
                return;
            }

            if (this.GetUser().password.Length < 6)
            {
                MessageBox.ErrorQuery("Error", "Minimum length of the password is 6 characters!", "OK");
                return;
            }

            if (new UserRepository(databasePath).GetByUsername(this.GetUser().username) != null)
            {
                MessageBox.ErrorQuery("Error", "User with the same username already exists!", "Change username");
                return;
            }
            Application.RequestStop();
        }
    }
}
