using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Charlie.OpenIam.Web.Areas.Identity.Pages.Account
{
    public class WwLoginModel : PageModel
    {
        public string AppId { get; set; }

        public string AgentId { get; set; }

        public string State { get; set; }

        public string RedirectUri { get; set; }

        public void OnGet(string appId, string agentId, string state, string redirect_uri)
        {
            AppId = appId;
            AgentId = agentId;
            State = state;
            RedirectUri = redirect_uri;
        }
    }
}
