using Microsoft.AspNetCore.Components;

namespace MusicApp.Components.Pages.Shared;

public partial class AppModal : ComponentBase
{
    [Parameter] public bool IsOpen { get; set; }
    [Parameter] public EventCallback<bool> IsOpenChanged { get; set; }
    [Parameter] public RenderFragment? ChildContent { get; set; }

    private async Task Close()
    {
        IsOpen = false;
        await IsOpenChanged.InvokeAsync(false);
    }

    private async Task OnBackdropClick()
    {
        await Close();
    }
}