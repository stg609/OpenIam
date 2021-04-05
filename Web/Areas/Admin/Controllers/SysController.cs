using System.Linq;
using System.Threading.Tasks;
using Charlie.OpenIam.Abstraction;
using Charlie.OpenIam.Core;
using Charlie.OpenIam.Core.Models;
using Charlie.OpenIam.Core.Models.Services.Dtos;
using Charlie.OpenIam.Core.Services.Abstractions;
using Charlie.OpenIam.Web.Infra;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Charlie.OpenIam.Web.Areas.Admin.Controllers
{
    /// <summary>
    /// 系统管理
    /// </summary>
    [UnitOfWork]
    [Area("Admin")]
    [Route("[area]/api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(AuthenticationSchemes = "Identity.Application")]
    [ApiController]
    public class SysController : ControllerBase
    {
        private readonly ISysService _sysService;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public SysController(ISysService sysService, SignInManager<ApplicationUser> signInManager)
        {
            _sysService = sysService;
            _signInManager = signInManager;
        }

        /// <summary>
        /// 获取系统信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [HasPermission(BuiltInPermissions.SYS_GET, true)]
        public async Task<ActionResult<SysDto>> Get()
        {
            var sys = await _sysService.GetAsync(); 
            var logins = (await _signInManager.GetExternalAuthenticationSchemesAsync());

            sys.AllQrExternalLogins = logins;

            return sys;
        }

        /// <summary>
        /// 更新系统信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [HasPermission(BuiltInPermissions.SYS_UPDATE, true)]
        public async Task<ActionResult> Update(SysDto model)
        {
            await _sysService.UpdateAsync(model);
            return Ok();
        }
    }
}
