using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Charlie.OpenIam.Common;
using Charlie.OpenIam.Core.Models;
using Charlie.OpenIam.Core.Models.Services.Dtos;
using Charlie.OpenIam.Core.Services.Abstractions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Charlie.OpenIam.Web.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ISysService _sysService;
        private readonly ILogger<LoginModel> _logger;
        private static readonly Regex _regexPhone;

        static LoginModel()
        {
            _regexPhone = new Regex(RegularExps.Phone);
        }

        public LoginModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ISysService sysService, ILogger<LoginModel> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _sysService = sysService;

            _logger = logger;
        }

        public readonly string JobNoLoginType = "jobNo";

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        /// 系统信息
        /// </summary>
        public SysDto SysInfo { get; private set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            public string Account { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "密码", Prompt = "密码")]
            public string Password { get; set; }

            [Display(Name = "自动登录")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl = returnUrl ?? Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            SysInfo = await _sysService.GetAsync();

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null, string type = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            SysInfo = await _sysService.GetAsync();
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                ApplicationUser user;
                if (type == JobNoLoginType && SysInfo.IsJobNoPwdLoginEnabled)
                {
                    // 工号登录
                    user = await _userManager.Users.SingleOrDefaultAsync(itm => itm.JobNo == Input.Account && itm.IsActive);
                    if (user == null)
                    {
                        ModelState.AddModelError(string.Empty, "账户名或密码不正确！");
                        return Page();
                    }
                }

                if (_regexPhone.IsMatch(Input.Account))
                {
                    if (SysInfo.IsPhonePwdLoginEnabled)
                    {
                        // 使用手机号登录
                        user = await _userManager.Users.SingleOrDefaultAsync(itm => itm.PhoneNumber == Input.Account && itm.IsActive);
                        if (user == null)
                        {
                            ModelState.AddModelError(string.Empty, "账户名或密码不正确！");
                            return Page();
                        }
                    }
                }

                // 使用用户名登录
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true

                user = await _userManager.Users.SingleOrDefaultAsync(itm => itm.NormalizedUserName == Input.Account.ToUpper() && itm.IsActive);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "账户名或密码不正确！");
                    return Page();
                }

                var result = await _signInManager.PasswordSignInAsync(user.UserName, Input.Password, Input.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
                    return LocalRedirect(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "账户名或密码不正确！");
                    return Page();
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
