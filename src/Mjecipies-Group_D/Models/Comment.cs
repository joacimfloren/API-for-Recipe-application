using Business.DataLayer;

namespace Mjecipies_Group_D.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public int Grade { get; set; }
        public string Image { get; set; }
        public int Created { get; set; }
        public string AccountId { get; set; }
        public int RecipeId { get; set; }
        public ApplicationUser Account { get; set; }
        public Recipe Recipe { get; set; }
    }
}