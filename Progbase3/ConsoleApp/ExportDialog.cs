using Terminal.Gui;

namespace ConsoleApp
{
    public class ExportDialog : Dialog
    {
        public bool canceled;
        private TextField filmTitleInput;
        private Label filePathLabel;

        public ExportDialog()
        {
            this.Title = "Export";

            Label infoLbl = new Label(2, 2, "Please, write film which reviews you want to export,\nand choose the directory!");
            this.Add(infoLbl);

            // film title
            Label filmTitleLbl = new Label(2, 6, "Film title:");
            filmTitleInput = new TextField("")
            {
                X = 20,
                Y = Pos.Top(filmTitleLbl),
                Width = 40,
            };
            this.Add(filmTitleLbl, filmTitleInput);

            // file path
            Label filePathLbl = new Label(2, 8, "File path:");
            filePathLabel = new Label(".")
            {
                X = 20,
                Y = Pos.Top(filePathLbl),
                Width = 40,
            };
            this.Add(filePathLbl, filePathLabel);

            // choose file btn
            Button chooseFilePathBtn = new Button("Open file")
            {
                X = Pos.Right(filmTitleInput) - 13,
                Y = Pos.Bottom(filmTitleInput) + 4,
            };
            chooseFilePathBtn.Clicked += OnChooseFilePath;
            this.Add(chooseFilePathBtn);

            // submit btn
            Button okBtn = new Button("OK")
            {
                X = Pos.Right(filmTitleInput) - 6,
                Y = Pos.Bottom(chooseFilePathBtn) + 1,
            };
            okBtn.Clicked += OnSubmit;
            this.Add(okBtn);

            // cancel btn
            Button cancelBtn = new Button("Cancel")
            {
                X = Pos.Right(filmTitleInput) - 17,
                Y = Pos.Top(okBtn),
            };
            cancelBtn.Clicked += OnCancel;
            this.Add(cancelBtn);
        }

        public string[] GetDataForExport()
        {
            string[] dataForExport = new string[2];
            dataForExport[0] = filmTitleInput.Text.ToString();
            dataForExport[1] = filePathLabel.Text.ToString();
            return dataForExport;
        }

        public void OnChooseFilePath()
        {
            OpenDialog dialog = new OpenDialog("Select file", "Open?");
            dialog.CanChooseDirectories = true;
            dialog.FilePath = "exported.xml";
            dialog.CanCreateDirectories = true;
            dialog.CanChooseFiles = false;
            Application.Run(dialog);

            if (!dialog.Canceled)
            {
                this.filePathLabel.Text = dialog.FilePath;
            }
        }

        public void OnSubmit()
        {
            this.canceled = false;
            if (filmTitleInput.Text == "" || filePathLabel.Text == "")
            {
                MessageBox.ErrorQuery("Error", "Choose directory for export and write a film title!", "OK");
                return;
            }

            if (!this.GetDataForExport()[1].EndsWith(".xml"))
            {
                int index = MessageBox.ErrorQuery("", "If you export in file, which extension is not `.xml`, data may be displayed incorrectly!", "Change extension", "OK");
                if (index == 0)
                {
                    return;
                }
            }
            Application.RequestStop();
        }

        public void OnCancel()
        {
            this.canceled = true;
            Application.RequestStop();
        }
    }
}
