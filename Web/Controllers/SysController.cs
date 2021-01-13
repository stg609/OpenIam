using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Charlie.OpenIam.Web.Controllers
{
    /// <summary>
    /// 系统 管理界面 Mvc 控制器
    /// </summary>
    [Authorize]
    public class SysController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
