using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Mjecipies_Group_D.Models
{
    public class Recipe
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Image { get; set; }

        public int Created { get; set; }

        public Account Creator { get; set; }

        public ICollection<Comment> Comments { get; set; }

        public ICollection<Direction> Directions { get; set; }

        [InverseProperty("FavoriteRecipes")]
        public ICollection<Account> FavoritedBy { get; set; }
    }
}