using System.Collections.Generic;
using Terminal.Gui;

namespace ConsoleApp
{
    public class FilmsListDialog : Dialog
    {
        private int pageLength = 5;
        private int page = 1;
        private string searchValue = "";

        private ListView allFilmsListView;
        private FrameView frameView;
        private Label currentPageLbl;
        private Label totalPagesLbl;
        private Button prevPageBtn;
        private Button nextPageBtn;
        private Label slash;
        private Label noFilmsLbl;
        private TextField searchInput;
        private FilmRepository filmRepo;
        private ReviewRepository reviewRepo;
        private FilmActorsRepository filmActorsRepo;

        public FilmsListDialog()
        {
            allFilmsListView = new ListView(new List<Film>())
            {
                Width = Dim.Fill(),
                Height = Dim.Fill()
            };
            allFilmsListView.OpenSelectedItem += OnOpenFilm;

            frameView = new FrameView("Films list")
            {
                X = 2,
                Y = 6,
                Width = Dim.Fill() - 4,
                Height = pageLength + 2,
            };
            frameView.Add(allFilmsListView);
            this.Add(frameView);

            // previous & next buttons
            prevPageBtn = new Button("<")
            {
                X = 2,
                Y = Pos.Bottom(frameView) + 2,
            };
            prevPageBtn.Clicked += OnPrevPage;
            this.Add(prevPageBtn);
            
            nextPageBtn = new Button(">")
            {
                X = Pos.Right(prevPageBtn) + 7,
                Y = Pos.Top(prevPageBtn),
            };
            nextPageBtn.Clicked += OnNextPage;
            this.Add(nextPageBtn);

            // pages
            currentPageLbl = new Label("?")
            {
                X = Pos.Right(prevPageBtn) + 1,
                Y = Pos.Top(prevPageBtn),
            };
            slash = new Label("/")
            {
                X = Pos.Right(currentPageLbl) + 1,
                Y = Pos.Top(prevPageBtn),
            };
            totalPagesLbl = new Label("?")
            {
                X = Pos.Right(slash) + 1,
                Y = Pos.Top(prevPageBtn),
            };
            this.Add(currentPageLbl, slash, totalPagesLbl);

            // if there are no films in the database
            noFilmsLbl = new Label(1, 1, "There are no films in the database!")
            {
                Visible = false,
            };
            frameView.Add(noFilmsLbl);

            Button backBtn = new Button("Back")
            {
                X = Pos.Right(frameView) - 9,
                Y = Pos.Bottom(frameView) + 2,
            };
            backBtn.Clicked += OnBack;
            this.Add(backBtn);

            // search input
            searchInput = new TextField(2, 4, 30, "");
            searchInput.KeyPress += OnSearchEnter;
            this.Add(searchInput);
        }

        public void SetRepository(FilmRepository filmRepo, FilmActorsRepository filmActorsRepo, ReviewRepository reviewRepo)
        {
            this.filmRepo = filmRepo;
            this.filmActorsRepo = filmActorsRepo;
            this.reviewRepo = reviewRepo;
            this.UpdCurrentPage();
        }

        public void OnSearchEnter(KeyEventEventArgs args)
        {
            if (args.KeyEvent.Key == Key.Enter)
            {
                this.searchValue = this.searchInput.Text.ToString();
                this.UpdCurrentPage();
            }
        }

        public void UpdCurrentPage()
        {
            int totalPages = filmRepo.GetSearchPagesCount(searchValue, pageLength);
            if (page > totalPages && page > 1)
            {
                page = totalPages;
            }
            this.currentPageLbl.Text = page.ToString();
            this.totalPagesLbl.Text = totalPages.ToString();
            this.allFilmsListView.SetSource(filmRepo.GetSearchPage(searchValue, page, pageLength));

            this.prevPageBtn.Visible = (page > 1);
            this.nextPageBtn.Visible = (page < filmRepo.GetTotalPages(pageLength));

            if (this.allFilmsListView.Source.ToList().Count == 0)
            {
                this.allFilmsListView.Visible = false;
                this.noFilmsLbl.Visible = true;
                this.currentPageLbl.Visible = false;
                this.totalPagesLbl.Visible = false;
                this.slash.Visible = false;
                // this.deleteProductBtn.Visible = false;
                // this.editProductBtn.Visible = false;
            }
            else
            {
                this.allFilmsListView.Visible = true;
                this.noFilmsLbl.Visible = false;
                this.currentPageLbl.Visible = true;
                this.totalPagesLbl.Visible = true;
                this.slash.Visible = true;
                // this.deleteProductBtn.Visible = true;
                // this.editProductBtn.Visible = true;
            }
        }

        public void OnOpenFilm(ListViewItemEventArgs args)
        {
            Film film = (Film)args.Value;
            OpenFilmDialog dialog = new OpenFilmDialog();
            dialog.SetFilm(film);

            Application.Run(dialog);

            if (dialog.deleted)
            {
                bool result = filmRepo.Delete(film.id);
                if (result)
                {
                    filmActorsRepo.DeleteFilm(film.id);
                    reviewRepo.DeleteAllFilmReviews(film.id);
                    if (page > filmRepo.GetTotalPages(pageLength) && page > 1)
                    {
                        page--;
                    }
                    allFilmsListView.SetSource(filmRepo.GetPage(page, pageLength));
                    this.UpdCurrentPage();
                }
                else
                {
                    MessageBox.ErrorQuery("Delete film", "Film cannot be deleted!", "OK");
                }
            }

            if (dialog.updated)
            {
                bool result = filmRepo.Update(film.id, dialog.GetFilm());
                if (result)
                {
                    allFilmsListView.SetSource(filmRepo.GetPage(page, pageLength));
                }
                else
                {
                    MessageBox.ErrorQuery("Update film", "Film cannot be updated!", "OK");
                }
            }
        }

        public void OnPrevPage()
        {
            if (page == 1)
            {
                return;
            }
            page--;
            this.UpdCurrentPage();
        }

        public void OnNextPage()
        {
            if (page >= filmRepo.GetTotalPages(pageLength))
            {
                return;
            }
            page++;
            this.UpdCurrentPage();
        }

        public void OnBack()
        {
            Application.RequestStop();
        }
    }
}
