using Microsoft.AspNetCore.Identity;
using MovieApp.Web.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieApp.Web.Models
{
    public class RoleDetails
    {
        public RoleDetails()
        {
            Role = new IdentityRole();
        }
        public IdentityRole Role { get; set; }
        public IEnumerable<AppIdentityUser> Members { get; set; }
        public IEnumerable<AppIdentityUser> NonMembers { get; set; }
    }

    public class RoleEditModel
    {
       
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public string[] IdsToAdd { get; set; }

        public string[] IdsToDelete { get; set; }


    }
}
