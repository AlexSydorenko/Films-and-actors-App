using Terminal.Gui;

namespace ConsoleApp
{
    public class ChooseWhatCreateDialog : Dialog
    {
        private string databasePath;

        private User user;
        private RadioGroup radioGroup;

        public ChooseWhatCreateDialog(string databasePath, User user)
        {
            this.databasePath = databasePath;
            this.user = user;

            this.Title = "Select";

            Label chooseVariantLbl = new Label(2, 5, "Select what you want to add:");
            this.Add(chooseVariantLbl);

            radioGroup = new RadioGroup(2, 7, new NStack.ustring[]{"Film", "Actor", "Review"});
            this.Add(radioGroup);

            Button backBtn = new Button(2, 11, "Back");
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

        public void OnBack()
        {
            Application.RequestStop();
        }

        public void OnSubmit()
        {
            int selected = radioGroup.SelectedItem;
            if (selected == 0)
            {
                CreateFilmDialog dialog = new CreateFilmDialog();
                Application.Run(dialog);

                if (!dialog.canceled)
                {
                    FilmRepository filmRepo = new FilmRepository(databasePath);
                    Film film = dialog.GetFilm();

                    if (filmRepo.GetByTitle(film.title) != null)
                    {
                        MessageBox.ErrorQuery("Error", "Film with such title is already exists in the database!", "OK");
                        return;
                    }

                    long filmId = filmRepo.Insert(film);
                    MessageBox.Query("", $"Film `{film.title}` was succesfully added to the database! It's id: {filmId}.", "OK");

                    Application.RequestStop();
                }
            }
            else if (selected == 1)
            {
                CreateActorDialog dialog = new CreateActorDialog();
                Application.Run(dialog);

                if (!dialog.canceled)
                {
                    ActorRepository actorRepo = new ActorRepository(databasePath);
                    FilmRepository filmRepo = new FilmRepository(databasePath);
                    FilmActorsRepository filmActorsRepo = new FilmActorsRepository(databasePath);
                    Actor actor = dialog.GetActor();

                    if (filmRepo.GetByTitle(actor.films[0].title) == null)
                    {
                        MessageBox.ErrorQuery("Error", "No such film in database. Add this film, and then add actor!", "OK");
                        return;
                    }

                    long actorId = actorRepo.Insert(actor);
                    Film film = filmRepo.GetByTitle(actor.films[0].title);
                    FilmActors filmActors = new FilmActors() { filmId = film.id, actorId = actorId };
                    filmActorsRepo.Insert(filmActors);

                    MessageBox.Query("", $"Actor `{actor.fullName}` was successfully added to the database! It's id: {actorId}.", "OK");
                    Application.RequestStop();
                }
            }
            else if (selected == 2)
            {
                CreateReviewDialog dialog = new CreateReviewDialog();
                Application.Run(dialog);

                if (!dialog.canceled)
                {
                    FilmRepository filmRepo = new FilmRepository(databasePath);
                    ReviewRepository reviewRepo = new ReviewRepository(databasePath);
                    Review review = dialog.GetReview();
                    Film film = filmRepo.GetByTitle(review.film.title);                    

                    if (film == null)
                    {
                        MessageBox.ErrorQuery("Error", "No such film in database. Add this film, and then add review!", "OK");
                        return;
                    }

                    if (reviewRepo.AuthorCantAddReview(this.user.id, film.id))
                    {
                        MessageBox.ErrorQuery("Error", "You have already written review to this film!", "OK");
                        return;
                    }

                    review.filmId = film.id;
                    review.userId = user.id;

                    reviewRepo.Insert(review);
                    MessageBox.Query("", "Your review was successfully added to the database!", "OK");
                    Application.RequestStop();
                }
            }
        }
    }
}
