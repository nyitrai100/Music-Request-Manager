using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MusicApp.Models;

namespace MusicApp.Components.Pages.Auth 
{
    public partial class Authentication(NavigationManager navigationManager) : ComponentBase
    {
        private AuthComponentType CurrentAuthComponent { get; set; } = AuthComponentType.Login;
        private void ShowComponent(AuthComponentType authComponent)
        {
            CurrentAuthComponent = authComponent;
            StateHasChanged();
        }
        protected override void OnInitialized()
        {
            var uri = navigationManager.ToAbsoluteUri(navigationManager.Uri);
            if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("type", out var type))
            {
                if (type == "register")
                {
                    CurrentAuthComponent = AuthComponentType.Register;
                    StateHasChanged();
                }
            }
        }
    }
}