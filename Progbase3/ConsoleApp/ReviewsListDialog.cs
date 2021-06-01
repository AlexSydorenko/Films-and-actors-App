using System.Collections.Generic;
using Terminal.Gui;

namespace ConsoleApp
{
    public class ReviewsListDialog : Dialog
    {
        private int pageLength = 5;
        private int page = 1;
        private string searchValue = "";

        private ListView allReviewsListView;
        private FrameView frameView;
        private Label currentPageLbl;
        private Label totalPagesLbl;
        private Button prevPageBtn;
        private Button nextPageBtn;
        private Label slash;
        private Label noReviewsLbl;
        private TextField searchInput;
        private ReviewRepository reviewRepo;
        private FilmRepository filmRepo;
        private UserRepository userRepo;

        public ReviewsListDialog()
        {
            allReviewsListView = new ListView(new List<Review>())
            {
                Width = Dim.Fill(),
                Height = Dim.Fill()
            };
            allReviewsListView.OpenSelectedItem += OnOpenReview;

            frameView = new FrameView("Reviews list")
            {
                X = 2,
                Y = 6,
                Width = Dim.Fill() - 4,
                Height = pageLength + 2,
            };
            frameView.Add(allReviewsListView);
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
            noReviewsLbl = new Label(1, 1, "There are no reviews in the database!")
            {
                Visible = false,
            };
            frameView.Add(noReviewsLbl);

            Button backBtn = new Button("Back")
            {
                X = Pos.Right(frameView) - 9,
                Y = Pos.Bottom(frameView) + 2,
            };
            backBtn.Clicked += OnBack;
            this.Add(backBtn);

            searchInput = new TextField(2, 4, 30, "");
            searchInput.KeyPress += OnSearchEnter;
            this.Add(searchInput);
        }

        public void SetRepository(ReviewRepository reviewRepo, FilmRepository filmRepo, UserRepository userRepo)
        {
            this.reviewRepo = reviewRepo;
            this.filmRepo = filmRepo;
            this.userRepo = userRepo;
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
            int totalPages = reviewRepo.GetSearchPagesCount(searchValue, pageLength);
            if (page > totalPages && page > 1)
            {
                page = totalPages;
            }
            this.currentPageLbl.Text = page.ToString();
            this.totalPagesLbl.Text = totalPages.ToString();
            List<Review> reviewsList = reviewRepo.GetSearchPage(searchValue, page, pageLength);
            foreach (Review review in reviewsList)
            {
                review.film = filmRepo.GetById(review.filmId);
                review.author = userRepo.GetById(review.userId);
            }
            this.allReviewsListView.SetSource(reviewsList);

            this.prevPageBtn.Visible = (page > 1);
            this.nextPageBtn.Visible = (page < reviewRepo.GetTotalPages(pageLength));

            if (this.allReviewsListView.Source.ToList().Count == 0)
            {
                this.allReviewsListView.Visible = false;
                this.noReviewsLbl.Visible = true;
                this.currentPageLbl.Visible = false;
                this.totalPagesLbl.Visible = false;
                this.slash.Visible = false;
                // this.deleteProductBtn.Visible = false;
                // this.editProductBtn.Visible = false;
            }
            else
            {
                this.allReviewsListView.Visible = true;
                this.noReviewsLbl.Visible = false;
                this.currentPageLbl.Visible = true;
                this.totalPagesLbl.Visible = true;
                this.slash.Visible = true;
                // this.deleteProductBtn.Visible = true;
                // this.editProductBtn.Visible = true;
            }
        }

        public void OnOpenReview(ListViewItemEventArgs args)
        {
            Review review = (Review)args.Value;
            OpenReviewDialog dialog = new OpenReviewDialog();
            review.film = filmRepo.GetById(review.filmId);
            review.author = userRepo.GetById(review.userId);
            dialog.SetReview(review);

            Application.Run(dialog);

            if (dialog.deleted)
            {
                bool result = reviewRepo.Delete(review.id);
                if (result)
                {
                    if (page > reviewRepo.GetTotalPages(pageLength) && page > 1)
                    {
                        page--;
                    }
                    List<Review> reviewsList = reviewRepo.GetPage(page, pageLength);
                    foreach (Review rev in reviewsList)
                    {
                        rev.film = filmRepo.GetById(review.filmId);
                        rev.author = userRepo.GetById(review.userId);
                    }
                    allReviewsListView.SetSource(reviewsList);
                    this.UpdCurrentPage();
                }
                else
                {
                    MessageBox.ErrorQuery("Delete review", "Review cannot be deleted!", "OK");
                }
            }

            if (dialog.updated)
            {
                bool result = reviewRepo.Update(review.id, dialog.GetReview());
                if (result)
                {
                    List<Review> reviewsList = reviewRepo.GetPage(page, pageLength);
                    foreach (Review rev in reviewsList)
                    {
                        rev.film = filmRepo.GetById(review.filmId);
                        rev.author = userRepo.GetById(review.userId);
                    }
                    allReviewsListView.SetSource(reviewsList);
                    this.UpdCurrentPage();
                }
                else
                {
                    MessageBox.ErrorQuery("Update review", "Review cannot be updated!", "OK");
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
            if (page >= reviewRepo.GetTotalPages(pageLength))
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
