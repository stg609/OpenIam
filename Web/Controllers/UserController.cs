using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Charlie.OpenIam.Web.Controllers
{
    /// <summary>
    /// 用户 管理界面 Mvc 控制器
    /// </summary>
    [Authorize]
    public class UserController : Controller
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

        /// <summary>
        /// 角色
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Roles()
        {
            return View();
        }

        /// <summary>
        /// 权限
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Permissions()
        {
            return View();
        }
    }
}
