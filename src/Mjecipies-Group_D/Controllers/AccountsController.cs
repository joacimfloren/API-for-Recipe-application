using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business.DataLayer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Mjecipies_Group_D.Business_Layer;
using Mjecipies_Group_D.Models;
using Microsoft.AspNetCore.Authorization;
using Mjecipies_Group_D.Models.ViewModels;

namespace Mjecipies_Group_D.Controllers
{
    [Authorize]
    [Route("api/v1/[controller]")]
    public class AccountsController : Controller
    {
        public UserManager<ApplicationUser> UserManager;

        public AccountsController(UserManager<ApplicationUser> am)
        {
            UserManager = am;
        }

        [AllowAnonymous]
        [HttpPost("password")]
        public async Task<IActionResult> CreateAccountWithPassword([FromBody]AccountViewModel avm)
        {
            List<string> errors = CheckLongituteAndLatitude(avm.Latitude, avm.Longitude);
            if (errors.Count != 0)
            {
                return BadRequest(errors);
            }
            var user = new ApplicationUser { UserName = avm.UserName ,Longitude = (double)avm.Longitude, Latitude = (double)avm.Latitude };
            var result = await UserManager.CreateAsync(user, avm.Password);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return CreatedAtRoute("GetAccount", new { user.Id }, user);
        }

        [HttpPost("facebook")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateAccountWithFacebook([FromBody]FacebookViewModel avm)
        {
            List<string> errors = CheckLongituteAndLatitude(avm.Latitude, avm.Longitude);
            if (avm.Token == null)
            {
                errors.Add("TokenMissing");
            }
            if (errors.Count != 0)
            {
                return BadRequest(errors);
            }
            Facebook fbOptions = new Facebook();
            FacebookResponse fbResponse = await fbOptions.GetFacebookResponse(avm.Token);
            var user = new ApplicationUser { UserName = avm.UserName, Longitude = (double)avm.Longitude, Latitude = (double)avm.Latitude, FacebookId = fbResponse.data.user_id.ToString()};
            var result = await UserManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return CreatedAtRoute("GetAccount", new { user.Id }, user);    
        }

        [HttpGet("{id}", Name = "GetAccount")]
        public async Task<IActionResult> GetAccount(string id)
        {
            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateAccount([FromBody]GpsViewModel avm, string id)
        {
            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            var tokenid = this.User.Claims.FirstOrDefault(c => c.Type == "UserId").Value;
            if (id != tokenid)
            {
                return Unauthorized();
            }
            if (avm != null)
            {
                if (avm.Longitude != null)
                {
                    user.Longitude = (double)avm.Longitude;
                }
                if (avm.Latitude != null)
                {
                    user.Latitude = (double)avm.Latitude;
                }
            }

            var result = await UserManager.UpdateAsync(user);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccount(string id)
        {
            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var tokenid = this.User.Claims.FirstOrDefault(c => c.Type == "UserId").Value;
            if (id != tokenid)
            {
                return Unauthorized();
            }

            RecipeManager rm = new RecipeManager();
            CommentManager cm = new CommentManager();

            var comments = user.Comments;
            var recipes = user.Recipes;
            if (comments != null)
            {
                foreach (var c in comments)
                {
                    cm.Delete(c.Id);
                }
            }

            if (recipes != null)
            {
                foreach (var r in recipes)
                {
                    rm.Delete(r.Id);
                }
            }

            var result = await UserManager.DeleteAsync(user);
            return NoContent();
        }

        [HttpGet("{id}/recipes")]
        public async Task<IActionResult> GetRecipes(string id)
        {
            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user.Recipes);
        }

        [HttpGet("{id}/favorites")]
        public async Task<IActionResult> GetFavorites(string id)
        {
            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user.FavoriteRecipes);
        }

        [HttpGet("{id}/comments")]
        public async Task<IActionResult> GetComments(string id)
        {
            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user.Comments);
        }

        [HttpPut("{id}/favorites")]
        public async Task<IActionResult> UpdateFavorites([FromBody] List<int> favoritIds, string id)
        { 
            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            if (this.User.Claims.FirstOrDefault(c => c.Type == "UserId").Value != id)
            {
                return Unauthorized();
            }

            if (favoritIds != null)
            {
                List<string> errorCodes = RecipesController.ControlRecipesIds(favoritIds);
                if (errorCodes.Count > 0)
                {
                    return BadRequest(errorCodes);
                }

                IFavoriteManager fm = new FavoriteManager();
                fm.Remove(id);
                fm.Add(favoritIds, id);             
            }

            await UserManager.UpdateAsync(user);
            return NoContent();
        }

        public List<string> CheckLongituteAndLatitude(double? lat, double? lon)
        {
            List<string> errors = new List<string>();
            if (lat == null) {
                errors.Add("LatitudeMissing");
            }
            if (lon == null) {
                errors.Add("LongitudeMissing");
            }

            return errors;
        }
    }
}