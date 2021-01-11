using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Charlie.OpenIam.Abstraction;
using Charlie.OpenIam.Common;
using Charlie.OpenIam.Core;
using Charlie.OpenIam.Core.Services.Abstractions;
using Charlie.OpenIam.Core.Services.Dtos;
using Charlie.OpenIam.Infra;
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
    /// Client 管理
    /// </summary>
    [UnitOfWork(typeof(IamConfigurationDbContext))]
    [Area("Admin")]
    [Route("[area]/api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(AuthenticationSchemes = "Identity.Application")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly IClientService _clientService;
        private readonly IMapper _mapper;

        /// <summary>
        /// 构造函数
        /// </summary>
        public ClientsController(IClientService clientService, IMapper mapper)
        {
            _clientService = clientService;
            _mapper = mapper;
        }

        /// <summary>
        /// 获取 Client 集合
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [HasPermission(BuiltInPermissions.CLIENT_GET, true)]
        public async Task<ActionResult<PaginatedDto<ClientDto>>> GetClients(string name = null, string clientId = null, int pageSize = 10, int pageIndex = 1)
        {
            pageSize = pageSize < 0 ? 0 : pageSize;
            pageIndex = pageIndex < 1 ? 1 : pageIndex;

            // 除了平台的超级管理员，其他管理员只能管理所拥有的 Client 的资源
            bool isSuper = User.IsSuperAdmin();
            IEnumerable<string> allowedClientIds = null;
            if (!isSuper)
            {
                allowedClientIds = User.FindAll(JwtClaimTypes.ClientId).Select(itm => itm.Value);
            }

            return await _clientService.GetAllAsync(name, new[] { clientId }, allowedClientIds, pageSize, pageIndex);
        }

        /// <summary>
        /// 获取某个 Client
        /// </summary>
        /// <returns></returns>
        [HasPermission(BuiltInPermissions.CLIENT_GET, true)]
        [HttpGet("{clientId}")]
        public async Task<ActionResult<ClientDto>> GetClient(string clientId)
        {
            bool isSuper = User.IsSuperAdmin();
            IEnumerable<string> allowedClientIds = null;
            if (!isSuper)
            {
                allowedClientIds = User.FindAll(JwtClaimTypes.ClientId).Select(itm => itm.Value);
            }

            return await _clientService.GetAsync(clientId, allowedClientIds);
        }

        /// <summary>
        /// 添加 Client 
        /// </summary>
        /// <returns></returns>
        [HasPermission(BuiltInPermissions.CLIENT_CREATE, true)]
        [HttpPost]
        public async Task<ActionResult<ClientDto>> AddClient(ClientNewViewModel model)
        {
            var result = await _clientService.CreateAsync(_mapper.Map<ClientNewDto>(model));
            return new CreatedAtActionResult(nameof(GetClient), nameof(ClientsController), new { result.ClientId }, new ClientDto
            {
                ClientId = result.ClientId,
                PlainSecret = result.PlainSecret,
                ClientName = model.ClientName,
                Description = model.Description
            });
        }

        /// <summary>
        /// 更新 Client 
        /// </summary>
        /// <returns></returns>
        [HasPermission(BuiltInPermissions.CLIENT_UPDATE, true)]
        [HttpPut("{clientId}")]
        public async Task<ActionResult> UpdateClient(string clientId, ClientUpdateDto model)
        {
            bool isSuper = User.IsSuperAdmin();
            IEnumerable<string> allowedClientIds = null;
            if (!isSuper)
            {
                allowedClientIds = User.FindAll(JwtClaimTypes.ClientId).Select(itm => itm.Value);
            }

            await _clientService.UpdateAsync(clientId, model, allowedClientIds);
            return Ok();
        }

        /// <summary>
        /// 启用/禁用
        /// </summary>
        /// <param name="clientId">clientId</param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HasPermission(BuiltInPermissions.CLIENT_UPDATE, true)]
        [HttpPut("{clientId}/enabled")]
        public async Task<ActionResult> EnableClient(string clientId, EnableClientViewModel model)
        {
            // 除了平台的超级管理员，其他管理员只能管理所拥有的 Client 的资源
            bool isSuper = User.IsSuperAdmin();
            IEnumerable<string> allowedClientIds = null;
            if (!isSuper)
            {
                allowedClientIds = User.FindAll(JwtClaimTypes.ClientId).Select(itm => itm.Value);
            }

            await _clientService.SwitchAsync(clientId, model.IsEnabled, allowedClientIds);
            return Ok();
        }

        /// <summary>
        /// 刷新 Client 的密钥
        /// </summary>
        /// <param name="clientId">clientId</param>
        /// <returns></returns>
        [HasPermission(BuiltInPermissions.CLIENT_UPDATE, true)]
        [HttpPost("{clientId}/secrets")]
        public async Task<ActionResult<string>> UpdateClientSecret(string clientId)
        {
            // 除了平台的超级管理员，其他管理员只能管理所拥有的 Client 的资源
            bool isSuper = User.IsSuperAdmin();
            IEnumerable<string> allowedClientIds = null;
            if (!isSuper)
            {
                allowedClientIds = User.FindAll(JwtClaimTypes.ClientId).Select(itm => itm.Value);
            }

            string secret = await _clientService.ResetSecretAsync(clientId, allowedClientIds);
            return secret;
        }

        /// <summary>
        /// 删除 Client
        /// </summary>
        /// <param name="model">要移除的clientId</param>
        /// <returns></returns>
        [HasPermission(BuiltInPermissions.CLIENT_DELETE, true)]
        [HttpDelete]
        public async Task<ActionResult> RemoveClient(ClientRemoveViewModel model)
        {
            bool isSuper = User.IsSuperAdmin();
            IEnumerable<string> allowedClientIds = null;
            if (!isSuper)
            {
                allowedClientIds = User.FindAll(JwtClaimTypes.ClientId).Select(itm => itm.Value);
            }

            await _clientService.RemoveAsync(model.ClientIds, allowedClientIds);
            return Ok();
        }      
    }
}