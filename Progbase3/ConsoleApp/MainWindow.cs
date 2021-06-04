using System.Collections.Generic;
using Terminal.Gui;
using System.IO;
using System.Threading;

namespace ConsoleApp
{
    public class MainWindow : Window
    {
        private string databasePath;
        private User user = null;
        private Button registerBtn;
        private Button loginBtn;
        private Button logoutBtn;
        private Label introLbl;

        public MainWindow(string databasePath)
        {
            this.databasePath = databasePath;
            this.Title = "KinoPoisk";

            MenuBar menu = new MenuBar(new MenuBarItem[] {
                new MenuBarItem ("_File", new MenuItem [] {
                    new MenuItem ("_Exit", "Quit the program", OnExit),
                    new MenuItem ("_Export", "Export all film reviews", OnExport),
                    new MenuItem ("_Import", "Import all film reviews", OnImport),
                }),
                new MenuBarItem ("_Help", new MenuItem [] {
                    new MenuItem ("_About", "", OnAbout),
                }),
            });
            this.Add(menu);

            introLbl = new Label(2, 6, "Hello!")
            {
                Width = 50,
            };
            this.Add(introLbl);

            Label infoLbl = new Label(2, 8, "Welcome to the one of the world's biggest films database \"KinoPoisk\"!\n" +
                "Here you can get ingormation about many popular films!\n" +
                "We'll appreciate if you write reviews on films you watched!");
            this.Add(infoLbl);

            Button createBtn = new Button("Add new")
            {
                X = 2,
                Y = 12,
            };
            createBtn.Clicked += OnCreate;
            this.Add(createBtn);

            Button showBtn = new Button("Show all")
            {
                X = Pos.Right(createBtn) + 3,
                Y = Pos.Top(createBtn),
            };
            showBtn.Clicked += OnShowAll;
            this.Add(showBtn);

            Button generateBtn = new Button("Generate report")
            {
                X = 2,
                Y = Pos.Bottom(createBtn) + 1,
            };
            generateBtn.Clicked += OnGenerate;
            this.Add(generateBtn);

            registerBtn = new Button("Sign up")
            {
                X = Pos.Right(this) - 28,
                Y = 2,
            };
            registerBtn.Clicked += OnSignUp;
            this.Add(registerBtn);

            loginBtn = new Button("Log in")
            {
                X = Pos.Right(this) - 15,
                Y = 2,
            };
            loginBtn.Clicked += OnLogIn;
            this.Add(loginBtn);

            logoutBtn = new Button("Log out")
            {
                X = Pos.Right(this) - 15,
                Y = 2,
                Visible = false,
            };
            logoutBtn.Clicked += OnLogOut;
            this.Add(logoutBtn);
        }

        public void OnSignUp()
        {
            RegistrationDialog dialog = new RegistrationDialog(databasePath);
            Application.Run(dialog);

            if (!dialog.canceled)
            {
                Authentication authentication = new Authentication(new UserRepository(databasePath));
                this.user = dialog.GetUser();
                long userId = authentication.RegisterUser(this.user);
                this.user.id = (int)userId;

                MessageBox.Query("", $"You have successfully registered in the system! Your unique id: `{this.user.id}`", "OK");
                this.loginBtn.Visible = false;
                this.registerBtn.Visible = false;
                this.logoutBtn.Visible = true;
                this.introLbl.Text = $"Hello, {this.user.username}!";
            }
        }

        public void OnLogIn()
        {
            LoginDialog dialog = new LoginDialog(databasePath);
            Application.Run(dialog);

            if (!dialog.canceled)
            {
                Authentication authentication = new Authentication(new UserRepository(databasePath));
                this.user = authentication.LoginUser(dialog.GetUserData()[0], dialog.GetUserData()[1]);
                if (this.user == null)
                {
                    MessageBox.ErrorQuery("Error", "Incorrect username or password!", "OK");
                    return;
                }
                MessageBox.Query("", $"You have successfully logged in the system!", "OK");
                this.loginBtn.Visible = false;
                this.registerBtn.Visible = false;
                this.logoutBtn.Visible = true;
                this.introLbl.Text = $"Hello, {this.user.username}!";
            }
        }

        public void OnLogOut()
        {
            int index = MessageBox.Query("Log Out", "Are you sure?", "No", "Yes");
            if (index == 0)
            {
                return;
            }
            this.user = null;
            this.loginBtn.Visible = true;
            this.registerBtn.Visible = true;
            this.logoutBtn.Visible = false;
            this.introLbl.Text = "Hello!";
        }

        public void OnAbout()
        {
            MessageBox.Query("About", "This is electronic database of films and actors!\nDeveloper: Sydorenko Oleksandr", "OK");
        }

        public void OnExit()
        {
            Application.RequestStop();
        }

        public void OnExport()
        {
            if (this.user == null)
            {
                MessageBox.ErrorQuery("", "Please, log in or register at first!", "OK");
                return;
            }

            ExportDialog dialog = new ExportDialog();
            Application.Run(dialog);

            if (!dialog.canceled)
            {
                FilmRepository filmRepo = new FilmRepository(databasePath);
                Film film = filmRepo.GetByTitle(dialog.GetDataForExport()[0]);
                if (film == null)
                {
                    MessageBox.ErrorQuery("Error", "No such film in database. Add this film, and then try to export it's reviews!", "OK");
                    return;
                }

                string filePath = dialog.GetDataForExport()[1];
                if (!File.Exists(filePath))
                {
                    File.Create(filePath).Close();
                }

                DataExportAndImport.ExportFilmReviews(new ReviewRepository(databasePath), film.id, filePath);
                MessageBox.Query("", $"All reviews on film `{film.title}` was succesfully exported to file `{filePath}`!", "OK");
            }
        }

        public void OnImport()
        {
            if (this.user == null)
            {
                MessageBox.ErrorQuery("", "Please, log in or register at first!", "OK");
                return;
            }

            if (this.user.role != "admin")
            {
                MessageBox.ErrorQuery("", "Only admin can do import into the database!", "OK");
                return;
            }

            OpenDialog dialog = new OpenDialog();
            Application.Run(dialog);

            if (!dialog.Canceled)
            {
                List<Review> filmReviews = new List<Review>();
                try
                {
                    filmReviews = DataExportAndImport.ImportFilmReviews(dialog.FilePath.ToString());
                }
                catch
                {
                    MessageBox.ErrorQuery("Error", "Choosen file has mistakes or information about different films!", "OK");
                    return;
                }

                FilmRepository filmRepo = new FilmRepository(databasePath);
                if (filmRepo.GetById(filmReviews[0].filmId) == null)
                {
                    MessageBox.ErrorQuery("Error", $"No film with such id `{filmReviews[0].filmId}` in the database!", "OK");
                    return;
                }

                ReviewRepository reviewRepo = new ReviewRepository(databasePath);
                foreach (Review review in filmReviews)
                {
                    if (!reviewRepo.Exists(review.userId, review.filmId))
                    {
                        reviewRepo.Insert(review);
                    }
                }

                MessageBox.Query("", $"All reviews on film `{filmRepo.GetById(filmReviews[0].filmId).title}` was succesfully imported to the database!", "OK");
            }
        }

        public void OnCreate()
        {
            if (this.user == null)
            {
                MessageBox.ErrorQuery("", "Please, log in or register at first!", "OK");
                return;
            }

            ChooseWhatCreateDialog dialog = new ChooseWhatCreateDialog(databasePath, user);
            Application.Run(dialog);
        }

        public void OnShowAll()
        {
            if (this.user == null)
            {
                MessageBox.ErrorQuery("", "Please, log in or register at first!", "OK");
                return;
            }

            ShowAllDialog dialog = new ShowAllDialog(databasePath, user);
            Application.Run(dialog);
        }

        public void OnGenerate()
        {
            if (this.user == null)
            {
                MessageBox.ErrorQuery("", "Please, log in or register at first!", "OK");
                return;
            }

            SelectFilmDialog dialog = new SelectFilmDialog(databasePath);
            Application.Run(dialog);

            if (!dialog.canceled)
            {
                ReportGenerator reportGenerator = new ReportGenerator(databasePath);
                Film film = new FilmRepository(databasePath).GetByTitle(dialog.GetFilmTitle());
                reportGenerator.GenerateReport(film);

                MessageBox.Query("", $"Report on the film `{dialog.GetFilmTitle()}` was successfully created!", "OK");
            }
        }
    }
}
