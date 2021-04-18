using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Charlie.OpenIam.Common;
using Charlie.OpenIam.Core.Models;
using Charlie.OpenIam.Core.Services.Abstractions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Charlie.OpenIam.Web.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ISysService _sysService;
        private readonly IUserService _userService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager, ISysService sysService, IUserService userService,
            ILogger<RegisterModel> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _sysService = sysService;
            _userService = userService;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            /// <summary>
            /// 用户名
            /// </summary>
            [Required]
            [Display(Name = "用户名(*)")]
            public string Username { get; set; }

            /// <summary>
            /// 手机号
            /// </summary>
            [Display(Name = "手机号")]
            public string Phone { get; set; }

            /// <summary>
            /// 内部系统人员编号
            /// </summary>
            [Display(Name = "内部系统的人员编号")]
            public string JobNo { get; set; }

            /// <summary>
            /// 邮箱
            /// </summary>
            [EmailAddress]
            [Display(Name = "邮箱")]
            public string Email { get; set; }

            /// <summary>
            /// 密码
            /// </summary>
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "密码(*)")]
            public string Password { get; set; }

            /// <summary>
            /// 确认密码
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "确认密码(*)")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            /// <summary>
            /// 头像
            /// </summary>
            [Display(Name = "头像")]
            public string Avatar { get; set; }

            /// <summary>
            /// 个人封面
            /// </summary>
            [Display(Name = "个人封面")]
            public string Cover { get; set; }

            /// <summary>
            /// 座右铭
            /// </summary>
            [Display(Name = "座右铭")]
            public string Motto { get; set; }

            /// <summary>
            /// Github 地址
            /// </summary>
            [Display(Name = "Github 地址")]
            public string Github { get; set; }

            /// <summary>
            /// 推特账号
            /// </summary>
            [Display(Name = "推特账号")]
            public string Twitter { get; set; }

            /// <summary>
            /// 新浪微博
            /// </summary>
            [Display(Name = "新浪微博")]
            public string SinaWeibo { get; set; }

            /// <summary>
            /// 个人备注
            /// </summary>
            [Display(Name = "个人备注")]
            public string Note { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            if (ModelState.IsValid)
            {
                var uid = await _userService.CreateAsync(new Core.Services.Dtos.UserNewDto
                {
                    Username = Input.Username,
                    Phone = Input.Phone,
                    JobNo = Input.JobNo,
                    Email = Input.Email,
                    Password = Input.Password
                });

                // 注册的用户只能是游客
                await _userService.AddRolesByRoleNameAsync(uid, new[] { Constants.ROLES_GUEST });

                return LocalRedirect(returnUrl);
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
