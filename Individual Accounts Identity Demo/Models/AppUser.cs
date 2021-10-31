using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Individual_Accounts_Identity_Demo.Models
{
    public class AppUser : IdentityUser
    {
        public Country Country { get; set; }

        public int Age { get; set; }

        [Required]
        public string Salary { get; set; }
    }
}
