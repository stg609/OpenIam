using System.Diagnostics;
using System.Threading.Tasks;
using Charlie.OpenIam.Abstraction;
using Charlie.OpenIam.Sdk.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IamApi _api;

        public HomeController(ILogger<HomeController> logger, IamApi api)
        {
            _logger = logger;
            _api = api;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HasPermission("webapp2.privacy")]
        //[Authorize]
        public async Task<IActionResult> Privacy()
        {
            var info = await _api.GetCurrentUserBasicInfoAsync();
            var result = await _api.HasPermissionAsync("webapp2.privacy", true);
            await _api.DeleteUserAsync("1");
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            return SignOut(CookieAuthenticationDefaults.AuthenticationScheme, OpenIdConnectDefaults.AuthenticationScheme);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public void ApiCall()
        {

        }
    }
}
