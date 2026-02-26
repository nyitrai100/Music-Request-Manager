using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using MusicApp.Models;

namespace MusicApp.Components.Pages.Auth
{
    public partial class Login : ComponentBase
    {
        protected override void OnInitialized()
        {
            var uri = Navigation.ToAbsoluteUri(Navigation.Uri);
            if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("error", out var error))
            {
                LoginModel.LoginError = error;
            }
        }
    }
}