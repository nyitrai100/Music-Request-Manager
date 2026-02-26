using DatabaseLayer.DbTables;
using Microsoft.AspNetCore.Components;
using MusicApp.Services;

namespace MusicApp.Components.Pages.AdminClubs;

public partial class CreateClub : ComponentBase
{
    [Inject] private AdminService AdminService { get; set; } = default!;
    [Parameter] public Clubs Club { get; set; } = default!;
    [Parameter] public EventCallback OnSaved { get; set; }

    private Clubs Model { get; set; } = default!;
    private string? Error { get; set; }

    protected override void OnInitialized()
    {
        Model = new Clubs
        {
            ClubName = Club.ClubName,
            Location = Club.Location,
            Floor = Club.Floor
        };
    }

    private async Task Save()
    {
        if (string.IsNullOrWhiteSpace(Model.ClubName))
        {
            Error = "Club name is required.";
            return;
        }

        var (success, error) = await AdminService.CreateClub(Model);

        if (!success)
        {
            Error = error;
            return;
        }

        await OnSaved.InvokeAsync();
    }

    private async Task Cancel()
    {
        await OnSaved.InvokeAsync();
    }
}