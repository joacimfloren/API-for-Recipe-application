using System;
using System.Collections.Generic;
using System.Linq;
using Business.DataLayer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Mjecipies_Group_D.Business_Layer
{
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        private static MjecipiesContext DbContext = new MjecipiesContext();

        public ApplicationUserManager(IUserStore<ApplicationUser> store, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<ApplicationUser> passwordHasher, IEnumerable<IUserValidator<ApplicationUser>> userValidators, IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<ApplicationUser>> logger, MjecipiesContext dbContext) : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
            DbContext = dbContext;
        }

        public static ApplicationUser GetUserWithFacebookId(string id)
        {
            try
            {
                return DbContext.Users.Where(u => u.FacebookId == id).FirstOrDefault();
            }
            catch (Exception)
            {
                throw;
            }
        } 
    }
}
