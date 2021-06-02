using Terminal.Gui;

namespace ConsoleApp
{
    public class EditUserDialog : Dialog
    {
        public bool canceled;
        public bool updated;

        private TextField usernameInput;
        private TextField fullNameInput;
        private TextField userRoleInput;
        private User user;

        public EditUserDialog(User user)
        {
            this.Title = "Edit user";
            this.user = user;

            int inputsColumnX = 20;
            int inputWidth = 40;

            Label infoLbl = new Label(2, 3, "You may change only user's role.");
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
            };
            this.Add(countryLbl, userRoleInput);

            // Cancel button
            Button cancelBtn = new Button("Cancel")
            {
                X = inputsColumnX + 30,
                Y = 13
            };
            cancelBtn.Clicked += OnCancel;
            this.Add(cancelBtn);

            // Submit btn
            Button okBtn = new Button("OK")
            {
                X = Pos.Left(cancelBtn) - 10,
                Y = 13
            };
            okBtn.Clicked += OnSubmit;
            this.Add(okBtn);
        }

        public void SetUser(User user)
        {
            this.usernameInput.Text = user.username;
            this.fullNameInput.Text = user.fullName;
            this.userRoleInput.Text = user.role;
        }

        public User GetUser()
        {
            return new User()
            {
                username = this.usernameInput.Text.ToString(),
                fullName = this.fullNameInput.Text.ToString(),
                role = this.userRoleInput.Text.ToString(),
            };
        }

        public void OnCancel()
        {
            this.canceled = true;
            Application.RequestStop();
        }

        public void OnSubmit()
        {
            this.canceled = false;
            if (this.userRoleInput.Text != "user" && this.userRoleInput.Text != "admin")
            {
                MessageBox.ErrorQuery("Error", "There are two types of user's role: admin or user", "OK");
                return;
            }
            Application.RequestStop();
        }
    }
}
