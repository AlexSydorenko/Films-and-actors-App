using System.Collections.Generic;
using Terminal.Gui;
using System.IO;

namespace ConsoleApp
{
    public class MainWindow : Window
    {
        // private ListView listView;
        private string databasePath;
        private User user;
        // private FilmRepository filmRepo;

        public MainWindow(string databasePath)
        {
            this.databasePath = databasePath;
            this.Title = "Films database";

            // CREATE USER HERE DEPENDING ON AUTHORIZED
            ///////////////////////////////////////////
            user = new User()
            {
                id = 1,
                fullName = "John McTominnay",
                username = "user007",
            };
            ///////////////////////////////////////////

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

            Button createBtn = new Button("Add new")
            {
                X = 2,
                Y = 6,
            };
            createBtn.Clicked += OnCreate;
            this.Add(createBtn);

            Button showBtn = new Button("Show all")
            {
                X = 2,
                Y = 8,
            };
            showBtn.Clicked += OnShowAll;
            this.Add(showBtn);
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
            ChooseWhatCreateDialog dialog = new ChooseWhatCreateDialog(databasePath, user);
            Application.Run(dialog);
        }

        public void OnShowAll()
        {
            ShowAllDialog dialog = new ShowAllDialog(databasePath, user);
            Application.Run(dialog);
        }
    }
}
