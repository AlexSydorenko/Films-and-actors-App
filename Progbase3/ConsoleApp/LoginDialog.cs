using Terminal.Gui;

namespace ConsoleApp
{
    public class LoginDialog : Dialog
    {
        private string databasePath;
        public bool canceled;

        private TextField usernameInput;
        private TextField passwordInput;

        public LoginDialog(string databasePath)
        {
            this.Title = "Log In";
            this.databasePath = databasePath;
            int inputWidth = 40;

            Label infoLbl = new Label("Please, enter your username and password")
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
            Label passwordLbl = new Label("Password:")
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

        public void OnCancel()
        {
            this.canceled = true;
            Application.RequestStop();
        }

        public void OnSubmit()
        {
            this.canceled = false;
            Application.RequestStop();
        }

        public string[] GetUserData()
        {
            string[] userData = new string[2];
            userData[0] = this.usernameInput.Text.ToString();
            userData[1] = this.passwordInput.Text.ToString();
            return userData;
        }
    }
}
