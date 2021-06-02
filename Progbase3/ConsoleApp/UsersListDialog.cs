using Terminal.Gui;
using System.Collections.Generic;

namespace ConsoleApp
{
    public class UsersListDialog : Dialog
    {
        private int pageLength = 5;
        private int page = 1;
        private string searchValue = "";

        private ListView allUsersListView;
        private FrameView frameView;
        private Label currentPageLbl;
        private Label totalPagesLbl;
        private Button prevPageBtn;
        private Button nextPageBtn;
        private Label slash;
        private Label noUsersLbl;
        private TextField searchInput;
        private UserRepository userRepo;
        private ReviewRepository reviewRepo;
        private User user;

        public UsersListDialog(User user)
        {
            this.user = user;

            allUsersListView = new ListView(new List<User>())
            {
                Width = Dim.Fill(),
                Height = Dim.Fill()
            };
            allUsersListView.OpenSelectedItem += OnOpenUser;

            frameView = new FrameView("Users list")
            {
                X = 2,
                Y = 6,
                Width = Dim.Fill() - 4,
                Height = pageLength + 2,
            };
            frameView.Add(allUsersListView);
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

            // if there are no users in the database
            noUsersLbl = new Label(1, 1, "There are no users in the database!")
            {
                Visible = false,
            };
            frameView.Add(noUsersLbl);

            Button backBtn = new Button("Back")
            {
                X = Pos.Right(frameView) - 9,
                Y = Pos.Bottom(frameView) + 2,
            };
            backBtn.Clicked += OnBack;
            this.Add(backBtn);

            // search input
            Label searchLbl = new Label(2, 4, "Search:");
            searchInput = new TextField("")
            {
                X = Pos.Right(searchLbl) + 1,
                Y = Pos.Top(searchLbl),
                Width = 30,
            };
            searchInput.KeyPress += OnSearchEnter;
            this.Add(searchLbl, searchInput);
        }

        public void SetRepository(UserRepository userRepo, ReviewRepository reviewRepo)
        {
            this.userRepo = userRepo;
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
            int totalPages = userRepo.GetSearchPagesCount(searchValue, pageLength);
            if (page > totalPages && page > 1)
            {
                page = totalPages;
            }
            this.currentPageLbl.Text = page.ToString();
            this.totalPagesLbl.Text = totalPages.ToString();
            this.allUsersListView.SetSource(userRepo.GetSearchPage(searchValue, page, pageLength));

            this.prevPageBtn.Visible = (page > 1);
            this.nextPageBtn.Visible = (page < userRepo.GetTotalPages(pageLength));

            if (this.allUsersListView.Source.ToList().Count == 0)
            {
                this.allUsersListView.Visible = false;
                this.noUsersLbl.Visible = true;
                this.currentPageLbl.Visible = false;
                this.totalPagesLbl.Visible = false;
                this.slash.Visible = false;
            }
            else
            {
                this.allUsersListView.Visible = true;
                this.noUsersLbl.Visible = false;
                this.currentPageLbl.Visible = true;
                this.totalPagesLbl.Visible = true;
                this.slash.Visible = true;
            }
        }

        public void OnOpenUser(ListViewItemEventArgs args)
        {
            User selectedUser = (User)args.Value;
            OpenUserDialog dialog = new OpenUserDialog(this.user);
            dialog.SetUser(selectedUser);

            Application.Run(dialog);

            if (dialog.deleted)
            {
                bool result = userRepo.Delete(selectedUser.username);
                if (result)
                {
                    reviewRepo.DeleteAllAuthorReviews(selectedUser.id);
                    if (page > userRepo.GetTotalPages(pageLength) && page > 1)
                    {
                        page--;
                    }
                    allUsersListView.SetSource(userRepo.GetPage(page, pageLength));
                    this.UpdCurrentPage();
                }
                else
                {
                    MessageBox.ErrorQuery("Delete user", "User cannot be deleted!", "OK");
                }
            }

            if (dialog.updated)
            {
                bool result = userRepo.Update(selectedUser.id, dialog.GetUser());
                if (result)
                {
                    allUsersListView.SetSource(userRepo.GetPage(page, pageLength));
                }
                else
                {
                    MessageBox.ErrorQuery("Update user", "User cannot be updated!", "OK");
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
            if (page >= userRepo.GetTotalPages(pageLength))
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
