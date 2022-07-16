using Microsoft.AspNetCore.Identity;
using MovieApp.Web.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieApp.Web.Infrastructure
{
    public class CustomPasswordValidator : IPasswordValidator<AppIdentityUser>
    {
        public Task<IdentityResult> ValidateAsync(UserManager<AppIdentityUser> manager, AppIdentityUser user, string password)
        {
            List<IdentityError> errors = new List<IdentityError>();

            if (password.ToLower().Contains(user.UserName.ToLower()))
            {
                errors.Add(new IdentityError()
                {
                    Code = "PasswordContainsUserName",
                    Description = "Password cannot contain username"
                });
            }

            if (password.Contains("123"))
            {
                errors.Add(new IdentityError()
                {
                    Code = "PasswordContainseSequence",
                    Description = "Password cannot contain numeric suquence"
                });
            }

            return Task.FromResult(errors.Count == 0 ?
                IdentityResult.Success :
                IdentityResult.Failed(errors.ToArray()));
        }
    }
}
