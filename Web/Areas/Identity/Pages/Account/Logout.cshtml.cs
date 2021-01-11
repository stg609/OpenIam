using System;
using System.Threading.Tasks;
using Charlie.OpenIam.Core.Models;
using IdentityModel;
using IdentityServer4.Events;
using IdentityServer4.Extensions;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Charlie.OpenIam.Web.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LogoutModel> _logger;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IEventService _events;

        [BindProperty]
        public string LogoutId
        {
            get; set;
        }

        public string PostLogoutRedirectUri { get; set; }
        public string ClientName { get; set; }
        public string SignOutIframeUrl { get; set; }

        public LogoutModel(SignInManager<ApplicationUser> signInManager, ILogger<LogoutModel> logger, IIdentityServerInteractionService interaction, IEventService events)
        {
            _signInManager = signInManager;
            _logger = logger;

            _interaction = interaction;
            _events = events;
        }

        public async Task<IActionResult> OnGetAsync(string logoutId)
        {
            LogoutId = logoutId;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // build a model so the logged out page knows what to display
            string idp = await BuildLoggedOutViewModelAsync(LogoutId);

            if (User?.Identity.IsAuthenticated == true)
            {
                // delete local authentication cookie
                await _signInManager.SignOutAsync();

                // raise the logout event
                await _events.RaiseAsync(new UserLogoutSuccessEvent(User.GetSubjectId(), User.GetDisplayName()));
            }

            if (!String.IsNullOrWhiteSpace(PostLogoutRedirectUri))
            {
                return Redirect(PostLogoutRedirectUri);
            }
            else
            {
                return LocalRedirect("~/");
            }
        }

        private async Task<string> BuildLoggedOutViewModelAsync(string logoutId)
        {
            // get context information (client name, post logout redirect URI and iframe for federated signout)
            var logout = await _interaction.GetLogoutContextAsync(logoutId);

            PostLogoutRedirectUri = logout?.PostLogoutRedirectUri;
            ClientName = string.IsNullOrEmpty(logout?.ClientName) ? logout?.ClientId : logout?.ClientName;
            SignOutIframeUrl = logout?.SignOutIFrameUrl;
            LogoutId = logoutId;

            if (User?.Identity.IsAuthenticated == true)
            {
                var idp = User.FindFirst(JwtClaimTypes.IdentityProvider)?.Value;
                if (idp != null && idp != IdentityServer4.IdentityServerConstants.LocalIdentityProvider)
                {
                    var providerSupportsSignout = await HttpContext.GetSchemeSupportsSignOutAsync(idp);
                    if (providerSupportsSignout)
                    {
                        if (LogoutId == null)
                        {
                            // if there's no current logout context, we need to create one
                            // this captures necessary info from the current logged in user
                            // before we signout and redirect away to the external IdP for signout
                            LogoutId = await _interaction.CreateLogoutContextAsync();
                        }

                        return idp;
                    }
                }
            }

            return null;
        }
    }
}
