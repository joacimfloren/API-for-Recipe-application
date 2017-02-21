using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mjecipies_Group_D.Business_Layer;
using Mjecipies_Group_D.Models.ViewModels;

namespace Mjecipies_Group_D.Controllers
{
    [Route("api/v1/[controller]")]
	public class CommentsController : Controller
	{
        public ICommentManager CommentManager;

        public CommentsController(ICommentManager cm)
        {
            CommentManager = cm; 
        }

        [HttpPatch("{id}")]
        public IActionResult UpdateComment([FromBody]CommentViewModel cvm, int id)
        {
            var comment = this.CommentManager.Get(id);
           
            if (comment == null )
            {
                return NotFound();
            }

            if (comment.AccountId != this.User.Claims.FirstOrDefault(c => c.Type == "UserId").Value)
            {
                return Unauthorized();
            }
            List<string> errorCodes = ControlComment(cvm, comment.RecipeId, comment.AccountId);

            if(errorCodes.Count > 0 )
            {
                return BadRequest(errorCodes);
            }
            this.CommentManager.Update(cvm, id);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteComment(int id)
        {
            var comment = this.CommentManager.Get(id); 
            if (comment == null)
            {
                return NotFound("404: Not found.");
            }

            var tokenid = this.User.Claims.FirstOrDefault(c => c.Type == "UserId").Value;
            if (tokenid != comment.AccountId)
            {
                return Unauthorized();
            }

            this.CommentManager.Delete(id);
            return NoContent();
        }

        [HttpPut("{id}/image")]
        public IActionResult UploadImage(int id, IFormFile image)
        {
            var comment = CommentManager.Get(id);
            if (comment == null)
            {
                return NotFound();
            }
            if (comment.AccountId != User.Claims.FirstOrDefault(c => c.Type == "UserId").Value)
            {
                return Unauthorized();
            }

            CommentManager.UploadImage(image, comment);
            return NoContent();
        }

        public static List<string> ControlComment(CommentViewModel comment, int recipeId, string commenterId)
        {
            List<string> errors = new List<string>();
            if (comment.Text == null)
            {
                errors.Add("TextMissing");
            }
            else if (comment.Text.Count() < 10 || comment.Text.Count() > 200)
            {
                errors.Add("TextWrongLength");
            }
            if (comment.Grade == default(int))
            {
                errors.Add("GradeMissing");
            }
            else if (comment.Grade < 1 || comment.Grade > 5)
            {
                errors.Add("GradeWrongValue");
            }
            if (commenterId == "")
            {
                errors.Add("CommenterIdMissing");
            }
            else
            {
                CommentManager cm = new CommentManager();
                if (cm.HasUserAlreadyCommented(commenterId, recipeId)) {
                    errors.Add("CommenterAlreadyCommented");
                }
            }
            return errors;
        }
    }
}
