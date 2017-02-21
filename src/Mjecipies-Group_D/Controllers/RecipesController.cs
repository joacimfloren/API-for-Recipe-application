using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Mjecipies_Group_D.Business_Layer;
using Mjecipies_Group_D.Models;
using Mjecipies_Group_D.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Business.DataLayer;
using Microsoft.AspNetCore.Http;

namespace Mjecipies_Group_D.Controllers
{
    [Route("api/v1/[controller]")]
    [Authorize]
    public class RecipesController : Controller
    {
        public IRecipeManager RecipeManager;
        public UserManager<ApplicationUser> UserManager;

        public RecipesController(IRecipeManager rm, UserManager<ApplicationUser> um)
        {
            RecipeManager = rm;
            UserManager = um;
        }

        [HttpPost()]
        public IActionResult CreateRecipe([FromBody]RecipeViewModel.Create r)
        {
            r.CreatorId = User.Claims.FirstOrDefault(c => c.Type == "UserId").Value;
            List<string> errorCodes = ControlRecipe(r);
            if (errorCodes.Count > 0)
            {
                return BadRequest(errorCodes);
            }
            
            Recipe recipe = RecipeManager.Add(r);
            return CreatedAtRoute("GetRecipe", new { recipe.Id }, recipe);
            //return CreatedAtRoute("CreateRecipe", recipe);
        }

        [AllowAnonymous]
        [HttpGet("{id}", Name = "GetRecipe")]
        public IActionResult GetRecipe(int id)
        {
            var recipe = RecipeManager.Get(id);
            if (recipe == null)
            {
                return NotFound("404: Not found.");
            }
            return Ok(recipe);
        }

        [AllowAnonymous]
        [HttpGet()]
        public IActionResult GetRecipesOnPage(int page)
        {
            if (page < 1)
            {
                return NotFound("404: Not Found.");
            }
            else
            {
                var recipeList = RecipeManager.GetPage(page);
                return Ok(recipeList);
            }
        }

        [AllowAnonymous]
        [HttpGet("{id}/comments", Name = "GetComment")]
        public IActionResult GetComments(int id)
        {
            var recipe = RecipeManager.Get(id);
            if (recipe == null)
            {
                return NotFound("404: Not found.");
            }

            var comments = RecipeManager.GetComments(id);
            return Ok(comments);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteRecipe(int id)
        {
            var recipe = RecipeManager.Get(id);
            if (recipe == null)
            {
                return NotFound("404: Not found.");
            }
            if (recipe.CreatorId != User.Claims.FirstOrDefault(c => c.Type == "UserId").Value)
            {
                return Unauthorized();
            }

            RecipeManager.Delete(id);
            return NoContent();
        }

        [AllowAnonymous]
        [HttpGet("search")]
        public IActionResult Search(string term)
        {
            if (term == null)
            {
                return BadRequest("[TermMissing]");
            }

            var searchResult = RecipeManager.Search(term);
            return Ok(searchResult);
        }

        [HttpPost("{id}/comments")]
        public IActionResult CreateComment([FromBody]CommentViewModel c, int id)
        {
            var recipes = RecipeManager.Get(id);
            if (recipes == null)
            {
                return NotFound("404: Not found.");
            }

            string commenterId = User.Claims.FirstOrDefault(cl => cl.Type == "UserId").Value;
            List<string> errorCodes = CommentsController.ControlComment(c, id, commenterId);
            if (errorCodes.Count > 0)
            {
                return BadRequest(errorCodes);
            }

            var comment = RecipeManager.AddComment(id, commenterId, c);
            //return CreatedAtRoute("CreateComment", RecipeManager.AddComment(id, commenterId, c));
            return CreatedAtRoute("GetComment", new { comment.Id }, comment);
        }

        [HttpPatch("{id}")]
        public IActionResult UpdateRecipe([FromBody]RecipeViewModel.Update rvm, int id)
        {
            var recipe = RecipeManager.Get(id);
            if (recipe == null)
            {
                return NotFound("404: Not found.");
            }
            if (recipe.CreatorId != User.Claims.FirstOrDefault(c => c.Type == "UserId").Value)
            {
                return Unauthorized();
            }
            List<string> errorCodes = ControlRecipeUpdate(rvm);
            if (errorCodes.Count > 0)
            {
                return BadRequest(errorCodes);
            }

            RecipeManager.Update(rvm, id);
            return NoContent();
        }

        [HttpPut("{id}/image")]
        public IActionResult UploadImage(int id, IFormFile image)
        {
            var recipe = RecipeManager.Get(id);
            if (recipe == null)
            {
                return NotFound();
            }
            if (recipe.CreatorId != User.Claims.FirstOrDefault(c => c.Type == "UserId").Value)
            {
                return Unauthorized();
            }

            RecipeManager.UploadImage(image, recipe);        

            return NoContent();
        }

        private static List<string> ControlRecipe(RecipeViewModel.Create recipe)
        {
            List<string> errors = new List<string>();

            if (recipe.Name == null)
            {
                errors.Add("NameMissing");
            }
            else if (recipe.Name.Count() < 5 || recipe.Name.Count() > 70)
            {
                errors.Add("NameWrongLength");
            }
            if (recipe.Description == null)
            {
                errors.Add("DescriptionMissing");
            }
            else if (recipe.Description.Count() < 10 || recipe.Description.Count() > 300)
            {
                errors.Add("DescriptionWrongLength");
            }
            if (recipe.Directions == null)
            {
                errors.Add("DirectionsMissing");
            }
            else
            {
                foreach (var d in recipe.Directions)
                {
                    if (d.Order == default(int))
                    {
                        errors.Add("DirectionOrderMissing");
                    }
                    if (d.Description == null)
                    {
                        errors.Add("DirectionDescriptionMissing");
                    }
                    else if (d.Description.Count() < 5 || d.Description.Count() > 120)
                    {
                        errors.Add("DirectionDescriptionWrongLength");
                    }
                }
            }

            return errors;
        }


        public static List<string> ControlRecipesIds(List<int> favoritIds)
        {
            List<string> errors = new List<string>();

            foreach(var id in favoritIds)
            {
                RecipeManager rm = new RecipeManager();
                var recipes = rm.Get(id);
                
                if( recipes == null)
                {
                    errors.Add("RecipeIdDoesNotExist");
                    break;
                } 
            }
            return errors;
        }

        private static List<string> ControlRecipeUpdate(RecipeViewModel.Update recipe)
        {
            List<string> errors = new List<string>();

            if (recipe.Name == null)
            {
                errors.Add("NameMissing");
            }
            else if (recipe.Name.Count() < 5 || recipe.Name.Count() > 70)
            {
                errors.Add("NameWrongLength");
            }
            if (recipe.Description == null)
            {
                errors.Add("DescriptionMissing");
            }
            else if (recipe.Description.Count() < 10 || recipe.Description.Count() > 300)
            {
                errors.Add("DescriptionWrongLength");
            }
            if (recipe.Directions == null)
            {
                errors.Add("DirectionsMissing");
            }
            else
            {
                foreach (var d in recipe.Directions)
                {
                    if (d.Order == default(int))
                    {
                        errors.Add("DirectionOrderMissing");
                    }
                    if (d.Description == null)
                    {
                        errors.Add("DirectionDescriptionMissing");
                    }
                    else if (d.Description.Count() < 5 || d.Description.Count() > 120)
                    {
                        errors.Add("DirectionDescriptionWrongLength");
                    }
                }
            }
            return errors;
        }
    }
}