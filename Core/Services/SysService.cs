using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Charlie.OpenIam.Core.Models.Repositories;
using Charlie.OpenIam.Core.Models.Services.Dtos;
using Charlie.OpenIam.Core.Services.Abstractions;

namespace Charlie.OpenIam.Core.Models.Services
{
    public class SysService : ISysService
    {
        private readonly ISysRepo _sysRepo;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public SysService(ISysRepo sysRepo, IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _sysRepo = sysRepo;
            _mapper = mapper;
        }

        public async Task<SysDto> GetAsync()
        {
            var sys = await _sysRepo.GetAsync();
            if (sys == null)
            {
                throw new IamException(System.Net.HttpStatusCode.InternalServerError, "系统信息不存在");
            }

            return _mapper.Map<SysDto>(sys);
        }

        public async Task UpdateAsync(SysDto model)
        {
            if (!model.IsJobNoUnique && model.IsJobNoPwdLoginEnabled)
            {
                throw new IamException(HttpStatusCode.BadRequest, "只有工号唯一才允许启用工号登录");
                // 必须确保当前数据库中的工号是唯一的
            }

            if (!model.IsUserPhoneUnique && model.IsPhonePwdLoginEnabled)
            {
                throw new IamException(HttpStatusCode.BadRequest, "只有手机号唯一才允许启用手机号登录");
                // 必须确保当前数据库中的工号是唯一的
            }

            if (model.IsJobNoUnique && !await _userService.IsJobNoUniqueAsync())
            {
                throw new IamException(HttpStatusCode.BadRequest, "当前系统中多个用户工号相同，无法设置成工号唯一");
            }

            if (model.IsUserPhoneUnique && !await _userService.IsPhoneUniqueAsync())
            {
                throw new IamException(HttpStatusCode.BadRequest, "当前系统中多个用户手机号相同，无法设置成手机号唯一");
            }

            var existed = await _sysRepo.GetAsync(false);
            if (existed != null)
            {
                existed.Update(model.IsJobNoUnique, model.IsUserPhoneUnique, model.IsPhonePwdLoginEnabled, model.IsJobNoPwdLoginEnabled, model.IsRegisterUserEnabled, model.EnabledQrExternalLogins);
            }
            else
            {
                _sysRepo.Add(new SystemInfo(model.IsJobNoUnique, model.IsUserPhoneUnique, model.IsPhonePwdLoginEnabled, model.IsJobNoPwdLoginEnabled, model.IsRegisterUserEnabled, model.EnabledQrExternalLogins));
            }
        }
    }
}
