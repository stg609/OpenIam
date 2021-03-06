using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Charlie.OpenIam.Web.Areas.Identity.Pages.Account
{
    public class DingTalkLoginModel : PageModel
    {
        public string AppId { get; set; }

        public string ResponseType { get; set; }

        public string Scope { get; set; }

        public string State { get; set; }

        public string RedirectUri { get; set; }

        public void OnGet(string appId, string response_type, string scope, string state, string redirect_uri)
        {
            AppId = appId;
            ResponseType = response_type;
            Scope = scope;
            State = state;
            RedirectUri = redirect_uri;
        }
    }
}
