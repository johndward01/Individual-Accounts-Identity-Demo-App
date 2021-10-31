using Individual_Accounts_Identity_Demo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Individual_Accounts_Identity_Demo.Controllers
{
    public class HomeController : Controller
    {
        private UserManager<AppUser> userManager;
        public HomeController(UserManager<AppUser> userMgr)
        {
            userManager = userMgr;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            AppUser user = await userManager.GetUserAsync(HttpContext.User);
            string message = $"Welcome {user.UserName}!";
            return View((object)message);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
