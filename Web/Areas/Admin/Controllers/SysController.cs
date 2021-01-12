using System.Threading.Tasks;
using Charlie.OpenIam.Abstraction;
using Charlie.OpenIam.Core;
using Charlie.OpenIam.Core.Models.Services.Dtos;
using Charlie.OpenIam.Core.Services.Abstractions;
using Charlie.OpenIam.Web.Infra;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
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

        public SysController(ISysService sysService)
        {
            _sysService = sysService;
        }

        /// <summary>
        /// 获取系统信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [HasPermission(BuiltInPermissions.SYS_GET, true)]
        public async Task<ActionResult<SysDto>> Get()
        {
            return await _sysService.GetAsync();
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
