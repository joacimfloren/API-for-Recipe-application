using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Mjecipies_Group_D.Models.ViewModels;
using Business.DataLayer;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Mjecipies_Group_D.Models;
using Mjecipies_Group_D.Business_Layer;
using System.Text;

namespace Mjecipies_Group_D.Controllers
{
    public class TokenProviderOptions
    {
        public string Path { get; set; } = "/api/v1/tokens/password";
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public TimeSpan Expiration { get; set; } = TimeSpan.FromMinutes(30);
        public SigningCredentials SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes("mineM0therfuck1ngSecretP@$$w0rd!")), SecurityAlgorithms.HmacSha256);
    }
   
    [Route("api/v1/[controller]")]
    public class TokensController : Controller
    {
        public UserManager<ApplicationUser> UserManager;
        private TokenProviderOptions Options;

        public TokensController(UserManager<ApplicationUser> am, IOptions<TokenProviderOptions> options)
        {
            UserManager = am;
            Options = options.Value;
        }

        [HttpPost("password")]
        public async Task<IActionResult> CreateTokenFromUserAndPassword([FromForm]TokenViewModel tvm)
        {
            var identity = await GetIdentity(tvm.username, tvm.password);
            if (identity == null)
            {             
                return BadRequest("Invalid username or password.");
            }

            return Ok(GenerateToken(identity.Id));
        }

        [HttpPost("facebook")]
        public async Task<IActionResult> CreateTokenFromFacebook([FromForm]FacebookViewModel fvm)
        {
            Facebook fb = new Facebook();
            FacebookResponse fbResponse = await fb.GetFacebookResponse(fvm.Token);
            
            var identity = ApplicationUserManager.GetUserWithFacebookId(Convert.ToString(fbResponse.data.user_id));
            if (identity == null)
            {
                return BadRequest("Invalid Facebook Id.");
            }

            return Ok(GenerateToken(identity.Id));
        }

        private async Task<ApplicationUser> GetIdentity(string username, string password)
        {
            var user = await UserManager.FindByNameAsync(username);
            if (user != null)
            {
                if (await UserManager.CheckPasswordAsync(user, password))
                {
                    return user;
                }
            }
                    
            return null;
        }

        private string GenerateToken(string id)
        {
            var now = DateTime.UtcNow;            
            var claims = new Claim[] { new Claim("UserId", id) };
       
            var jwt = new JwtSecurityToken(
                issuer: Options.Issuer,
                audience: Options.Audience,
                claims: claims,
                notBefore: now,
                expires: now.Add(Options.Expiration),
                signingCredentials: Options.SigningCredentials);
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var response = new
            {
                access_token = encodedJwt,
                expires_in = (int)Options.Expiration.TotalSeconds
            };

            return JsonConvert.SerializeObject(response, new JsonSerializerSettings { Formatting = Formatting.Indented });
        }
    }
}