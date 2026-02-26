using System.Net;
using Microsoft.AspNetCore.Components;
using MusicApp.Models;
using MusicApp.Services;

namespace MusicApp.Components.Pages.Auth
{
    public partial class Edit : ComponentBase
    {
        [Parameter] public EditModel Model { get; set; } = default!;
        [Parameter] public EventCallback OnSaved { get; set; }
        [Parameter] public EventCallback OnCancel { get; set; }
        [Inject] private AdminService AdminService { get; set; } = default!;
        
        private async Task HandleEdit()
        {
            Model.EditError = null;

            if (!string.IsNullOrEmpty(Model.Password) &&
                Model.Password != Model.ConfirmPassword)
            {
                Model.EditError = "Passwords do not match";
                return;
            }

            var result = await AdminService.EditUser(Model);

            if (result.Success)
                await OnSaved.InvokeAsync();
            else
                Model.EditError = result.Error;
        }
    }
}