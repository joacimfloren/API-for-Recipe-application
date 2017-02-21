using System.Collections.Generic;
using Business.DataLayer;

namespace Mjecipies_Group_D.Models
{
    public class Recipe
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Image { get; set; }

        public int Created { get; set; }
  
        public string CreatorId { get; set; }

        public ApplicationUser Creator { get; set; }

        public ICollection<Comment> Comments { get; set; }

        public ICollection<Direction> Directions { get; set; }

        public ICollection<Favorites> FavoritedBy { get; set; }
    }
}