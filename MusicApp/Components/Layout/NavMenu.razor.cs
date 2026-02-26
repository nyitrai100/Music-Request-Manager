using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MusicApp.Services;

namespace MusicApp.Components.Layout
{
    public partial class NavMenu : ComponentBase
    {
        [Inject] private HttpClient Http { get; set; } = default!;
        [Inject] private NavigationManager Navigation { get; set; } = default!;
        [Inject] private AuthenticationStateProvider AuthProvider { get; set; } = default!;
        [Parameter] public string? UserName { get; set; }
        
    }
}
