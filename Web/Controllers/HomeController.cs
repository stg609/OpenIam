using System.Threading.Tasks;
using Charlie.OpenIam.Core.Services.Abstractions;
using Charlie.OpenIam.Web.ViewModels;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Charlie.OpenIam.Web.Controllers
{
    /// <summary>
    /// 管理界面首页控制器
    /// </summary>
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUserService _userService;

        public HomeController(ILogger<HomeController> logger, IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            var result = await _userService.IsAdminAsync(User.FindFirst(JwtClaimTypes.Subject)?.Value);
            return View(new HomeIndexViewModel
            {
                IsSuperAdmin = result.IsSuperAdmin,
                IsAdmin = result.IsAdmin
            });
        }
    }
}
