namespace ConsoleApp
{
    public class EditFilmDialog : CreateFilmDialog
    {
        public EditFilmDialog()
        {
            this.Title = "Edit film";
        }

        public void SetFilm(Film film)
        {
            this.titleInput.Text = film.title;
            this.releaseYearInput.Text = film.releaseYear.ToString();
            this.countryInput.Text = film.country;
            this.directorInput.Text = film.director;
        }
    }
}