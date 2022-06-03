using System.ComponentModel.DataAnnotations;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Sphere.Auth.API.Pages.Ciba;

[SecurityHeaders]
[Authorize]
public class AllModel : PageModel
{
    public IEnumerable<BackchannelUserLoginRequest> Logins { get; set; } = Enumerable.Empty<BackchannelUserLoginRequest>();

    [BindProperty, Required(AllowEmptyStrings = false)]
    public string Id { get; set; } = string.Empty;

    [BindProperty, Required(AllowEmptyStrings = false)]
    public string Button { get; set; } = string.Empty;

    private readonly IBackchannelAuthenticationInteractionService backchannelAuthenticationInteraction;

    public AllModel(IBackchannelAuthenticationInteractionService backchannelAuthenticationInteractionService)
    {
        backchannelAuthenticationInteraction = backchannelAuthenticationInteractionService;
    }

    public async Task OnGet()
    {
        Logins = await backchannelAuthenticationInteraction.GetPendingLoginRequestsForCurrentUserAsync();
    }
}
