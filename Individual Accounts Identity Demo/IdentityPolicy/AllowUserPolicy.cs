using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Individual_Accounts_Identity_Demo.IdentityPolicy
{
    public class AllowUserPolicy : IAuthorizationRequirement
    {
        public string[] AllowUsers { get; set; }

        public AllowUserPolicy(params string[] users)
        {
            AllowUsers = users;
        }
    }
}
