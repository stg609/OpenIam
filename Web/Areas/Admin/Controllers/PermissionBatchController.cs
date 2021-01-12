using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Charlie.OpenIam.Abstraction;
using Charlie.OpenIam.Core;
using Charlie.OpenIam.Core.Services.Abstractions;
using Charlie.OpenIam.Core.Services.Dtos;
using Charlie.OpenIam.Web.Areas.Admin.ViewModels;
using Charlie.OpenIam.Web.Helpers;
using Charlie.OpenIam.Web.Infra;
using IdentityModel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Charlie.OpenIam.Web.Areas.Admin.Controllers
{
    /// <summary>
    /// 权限批量管理
    /// </summary>
    [UnitOfWork]
    [Area("Admin")]
    [Route("[area]/api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(AuthenticationSchemes = "Identity.Application")]
    public class PermissionBatchController : ControllerBase
    {
        private readonly IPermissionService _permService;
        private readonly IMapper _mapper;

        public PermissionBatchController(IPermissionService permService, IMapper mapper)
        {
            _permService = permService;
            _mapper = mapper;
        }

        /// <summary>
        /// 批量同步权限(仅可用于当前的 Client），会覆盖原先的权限
        /// </summary>
        /// <param name="permissions">需要同步的权限集合</param>
        /// <returns></returns>
        [HasPermission(BuiltInPermissions.PERM_SYNC)]
        [HttpPut]
        public async Task<ActionResult> SyncPermissions(SyncPermissionViewModel permissions)
        {
            // 除了平台的超级管理员，其他管理员只能管理所属 Client 的资源
            bool isSuper = User.IsSuperAdmin();
            IEnumerable<string> allowedClientIds = null;
            if (!isSuper)
            {
                allowedClientIds = User.FindAll(JwtClaimTypes.ClientId).Select(itm => itm.Value);
            }

            await _permService.SyncPermissionsAsync(_mapper.Map<SyncPermissionDto>(permissions), allowedClientIds);

            return Ok();
        }
    }
}
