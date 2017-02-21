using System;
using System.Collections.Generic;
using System.Linq;
using Business.DataLayer;
using Mjecipies_Group_D.Models;

namespace Mjecipies_Group_D.Business_Layer
{
    public class FavoriteManager : IFavoriteManager
    {
        private MjecipiesContext DbContext = new MjecipiesContext();

        public void Add(List<int> RecipiesIds, string UserId)
        {
            foreach (var id in RecipiesIds)
            {
                Favorites favorite = new Favorites();
                favorite.AccountId = UserId;
                favorite.RecipeId = id;
                DbContext.Add(favorite);
            }

            DbContext.SaveChanges();
        }

        public void Remove(string id)
        {
            try
            {             
                var favorites = DbContext.Favorites.Where(f => f.AccountId == id).ToList();
                foreach (var f in favorites)
                {
                    DbContext.Remove(f);
                }

                DbContext.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
