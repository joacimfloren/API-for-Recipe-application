using Mjecipies_Group_D.Models;
using Mjecipies_Group_D.Models.ViewModels;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace Mjecipies_Group_D.Business_Layer
{
    public interface IMjecipiesInterface
    {
    }
    public interface IRecipeManager : IMjecipiesInterface
    {
        Recipe Add(RecipeViewModel.Create r);
        Recipe Get(int id);
        List<Recipe> GetPage(int page, int perPage = 10);
        List<Comment> GetComments(int id);
        List<Recipe> Search(string term);
        void Delete(int id);
        Comment AddComment(int id, string accountId, CommentViewModel comment);
        Recipe Update(RecipeViewModel.Update recipe, int id);
        void UploadImage(IFormFile image, Recipe r);
    }

    public interface ICommentManager : IMjecipiesInterface
    {
        Comment Get(int id);
        void Delete(int id);
        void Update(CommentViewModel cvm, int id);
        bool HasUserAlreadyCommented(string commenterId, int recipeId);
        void UploadImage(IFormFile image, Comment r);
    }

    public interface IFavoriteManager : IMjecipiesInterface
    {
        void Add(List<int> RecipiesIds, string UserId);
        void Remove(string id);
    }
}
