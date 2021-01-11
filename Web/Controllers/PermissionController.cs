using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Charlie.OpenIam.Web.Controllers
{
    /// <summary>
    /// 权限 管理界面 Mvc 控制器
    /// </summary>
    [Authorize]
    public class PermissionController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 获取详情页
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Details(string id)
        {
            return View();
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
    }
}
