using Business.DataLayer;

namespace Mjecipies_Group_D.Models
{
    public class Favorites
    {
        public string AccountId { get; set; }
        public ApplicationUser Account { get; set; }
        public int RecipeId { get; set; }
        public Recipe Recipe { get; set; }

        public Favorites() {}
    }
}
