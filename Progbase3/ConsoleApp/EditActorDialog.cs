namespace ConsoleApp
{
    public class EditActorDialog : CreateActorDialog
    {
        public EditActorDialog()
        {
            this.Title = "Edit actor";

            this.filmInWhichStarredLbl.Visible = false;
            this.filmInWhichStarredInput.Visible = false;
        }

        public void SetActor(Actor actor)
        {
            this.fullnameInput.Text = actor.fullName;
            this.ageInput.Text = actor.age.ToString();
            this.residenceInput.Text = actor.residence;
        }

        public override Actor GetActor()
        {
            Actor actor = new Actor();
            if (fullnameInput.Text == "" || !int.TryParse(ageInput.Text.ToString(), out actor.age) || residenceInput.Text == "")
            {
                return null;
            }
            actor.fullName = fullnameInput.Text.ToString();
            actor.residence = residenceInput.Text.ToString();
            // Film film = new Film() { title = filmInWhichStarredInput.Text.ToString() };
            // actor.films.Add(film);
            return actor;
        }
    }
}
