using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Mjecipies_Group_D.Models
{
    public class Account
    {
        public string Id { get; set; }

        public string UserName { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public ICollection<Comment> Comments { get; set; }

        [InverseProperty("Creator")]
        public ICollection<Recipe> Recipes { get; set; }

        [InverseProperty("FavoritedBy")]
        public ICollection<Recipe> FavoriteRecipes { get; set; }
    }
}
