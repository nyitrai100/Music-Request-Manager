using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.WebUtilities;

namespace MusicApp.Components.Pages
{
    public partial class Index : LayoutComponentBase
    {
        [Inject] protected NavigationManager Nav { get; set; } = default!;
        [Inject] protected IJSRuntime Js { get; set; } = default!;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender) return;

            var uri = Nav.ToAbsoluteUri(Nav.Uri);
            var query = QueryHelpers.ParseQuery(uri.Query);

            if (query.TryGetValue("clubId", out var clubId))
            {
                await Js.InvokeVoidAsync(
                    "localStorage.setItem",
                    "selectedClubId",
                    clubId.ToString()
                );
            }
        }
    }
}