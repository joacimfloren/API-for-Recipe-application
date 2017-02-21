namespace Mjecipies_Group_D.Models
{
    public class Direction
    {
        public int Id { get; set; }
        public int Order { get; set; }
        public string Description { get; set; }
        public int RecipeId { get; set; }
        public Recipe Recipe { get; set; }
    }
}
