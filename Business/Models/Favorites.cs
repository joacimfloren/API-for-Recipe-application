using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mjecipies_Group_D.Models
{
    public class Favorites
    {
        private int _favoriteId;
        private int _recipeId;
        private string _accountId;

        public Favorites(int favoriteId, int recipeId, string accountId)
        {
            _favoriteId = favoriteId;
            _recipeId = recipeId;
            _accountId = accountId;
        }

        public int FavoriteId
        {
            get { return _favoriteId; }
            set { _favoriteId = value; }
        }

        public int RecipeId
        {
            get { return _recipeId; }
            set { _recipeId = value; }
        }

        public string AccountId
        {
            get { return _accountId; }
            set { _accountId = value; }
        }
    }
}
