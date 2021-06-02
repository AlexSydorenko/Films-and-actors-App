using System.Collections.Generic;
using Terminal.Gui;

namespace ConsoleApp
{
    public class ActorsListDialog : Dialog
    {
        private int pageLength = 5;
        private int page = 1;
        private string searchValue = "";

        private ListView allActorsListView;
        private FrameView frameView;
        private Label currentPageLbl;
        private Label totalPagesLbl;
        private Button prevPageBtn;
        private Button nextPageBtn;
        private Label slash;
        private Label noActorsLbl;
        private TextField searchInput;
        private ActorRepository actorRepo;
        private FilmActorsRepository filmActorsRepo;
        private User user;

        public ActorsListDialog(User user)
        {
            this.user = user;

            allActorsListView = new ListView(new List<Actor>())
            {
                Width = Dim.Fill(),
                Height = Dim.Fill()
            };
            allActorsListView.OpenSelectedItem += OnOpenActor;

            frameView = new FrameView("Actors list")
            {
                X = 2,
                Y = 6,
                Width = Dim.Fill() - 4,
                Height = pageLength + 2,
            };
            frameView.Add(allActorsListView);
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

            // if there are no actorss in the database
            noActorsLbl = new Label(1, 1, "There are no actors in the database!")
            {
                Visible = false,
            };
            frameView.Add(noActorsLbl);

            Button backBtn = new Button("Back")
            {
                X = Pos.Right(frameView) - 9,
                Y = Pos.Bottom(frameView) + 2,
            };
            backBtn.Clicked += OnBack;
            this.Add(backBtn);

            // search
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

        public void SetRepository(ActorRepository actorRepo, FilmActorsRepository filmActorsRepo)
        {
            this.actorRepo = actorRepo;
            this.filmActorsRepo = filmActorsRepo;
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
            int totalPages = actorRepo.GetSearchPagesCount(searchValue, pageLength);
            if (page > totalPages && page > 1)
            {
                page = totalPages;
            }
            this.currentPageLbl.Text = page.ToString();
            this.totalPagesLbl.Text = totalPages.ToString();
            this.allActorsListView.SetSource(actorRepo.GetSearchPage(searchValue, page, pageLength));

            this.prevPageBtn.Visible = (page > 1);
            this.nextPageBtn.Visible = (page < actorRepo.GetTotalPages(pageLength));

            if (this.allActorsListView.Source.ToList().Count == 0)
            {
                this.allActorsListView.Visible = false;
                this.noActorsLbl.Visible = true;
                this.currentPageLbl.Visible = false;
                this.totalPagesLbl.Visible = false;
                this.slash.Visible = false;
            }
            else
            {
                this.allActorsListView.Visible = true;
                this.noActorsLbl.Visible = false;
                this.currentPageLbl.Visible = true;
                this.totalPagesLbl.Visible = true;
                this.slash.Visible = true;
            }
        }

        public void OnOpenActor(ListViewItemEventArgs args)
        {
            Actor actor = (Actor)args.Value;
            OpenActorDialog dialog = new OpenActorDialog(this.user);
            dialog.SetActor(actor);

            Application.Run(dialog);

            if (dialog.deleted)
            {
                bool result = actorRepo.Delete(actor.id);
                if (result)
                {
                    filmActorsRepo.DeleteActor(actor.id);
                    if (page > actorRepo.GetTotalPages(pageLength) && page > 1)
                    {
                        page--;
                    }
                    allActorsListView.SetSource(actorRepo.GetPage(page, pageLength));
                    this.UpdCurrentPage();
                }
                else
                {
                    MessageBox.ErrorQuery("Delete actor", "Actor cannot be deleted!", "OK");
                }
            }

            if (dialog.updated)
            {
                bool result = actorRepo.Update(actor.id, dialog.GetActor());
                if (result)
                {
                    allActorsListView.SetSource(actorRepo.GetPage(page, pageLength));
                }
                else
                {
                    MessageBox.ErrorQuery("Update actor", "Actor cannot be updated!", "OK");
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
            if (page >= actorRepo.GetTotalPages(pageLength))
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
