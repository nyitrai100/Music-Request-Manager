using DatabaseLayer.DbTables;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using MusicApp.Controllers;
using MusicApp.Models;
using MusicApp.Services;

namespace MusicApp.Components.Pages
{
    public partial class Admin : ComponentBase, IDisposable
    {
        [Inject] private AdminService AdminService { get; set; } = default!;
        [Inject] private IJSRuntime Js { get; set; } = default!;
        [Inject] private DjService DjService { get; set; } = default!;
        [Inject] private AuthenticationStateProvider AuthStateProvider { get; set; } = default!;
        private List<UserDto>? Users { get; set; }
        private List<DjSets>? DjSets { get; set; }
        private IEnumerable<Songs>? DiagramData { get; set; }
        private List<Clubs>? Clubs { get; set; }
        private bool IsLoading { get; set; } = true;
        private bool _firstRender = true;
        private bool ShowRegisterDialog { get; set; } = false;
        private bool ShowEditUserDialog { get; set; } = false;
        private EditModel? SelectedUser { get; set; }
        private bool ShowNewPerformanceDialog { get; set; } = false;
        private bool ShowEditPerformanceDialog { get; set; } = false;
        private DjSets? SelectedPerformance { get; set; }
        private System.Timers.Timer? _pollingTimer;
        private const int PollingInterval = 5000;
        private bool _isPolling;
        private string _diagramKey = Guid.NewGuid().ToString();
        private string? _selectedClubName;
        private int CurrentUserPage { get; set; } = 1;
        private int PageUserSize { get; set; } = 5;
        private int CurrentClubPage { get; set; } = 1;
        private int ClubPageSize { get; set; } = 5;
        private bool ShowCreateClubDialog { get; set; }
        private bool ShowEditClubDialog { get; set; }
        private Clubs? SelectedClub { get; set; }

        private IEnumerable<UserDto> PagedUsers => 
            Users == null
                ? Enumerable.Empty<UserDto>()
                : Users.Skip((CurrentUserPage - 1) * PageUserSize).Take(PageUserSize);

        private int TotalUserPages => Users == null ? 1 : (int)Math.Ceiling((double)Users.Count / PageUserSize);
        private int CurrentPerformancePage { get; set; } = 1;
        private int PerformancePageSize { get; set; } = 10;
        private IEnumerable<DjSets> PagedPerformances =>
            DjSets == null
                ? Enumerable.Empty<DjSets>()
                : DjSets.Skip((CurrentPerformancePage - 1) * PerformancePageSize)
                    .Take(PerformancePageSize);
        private int TotalPerformancePages =>
            DjSets == null ? 1 : (int)Math.Ceiling((double)DjSets.Count / PerformancePageSize);
        
        private IEnumerable<Clubs> PagedClubs =>
            Clubs == null
                ? Enumerable.Empty<Clubs>()
                : Clubs.Skip((CurrentClubPage - 1) * ClubPageSize)
                    .Take(ClubPageSize);

        private int TotalClubPages =>
            Clubs == null ? 1 : (int)Math.Ceiling((double)Clubs.Count / ClubPageSize);
        
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!_firstRender)
                return;

            _firstRender = false;
            IsLoading = true;
            
            Users = await AdminService.GetAllUsers();
            DjSets = await AdminService.GetAllDjSets();
            Clubs =  await AdminService.GetAllClubs();
            await Task.Delay(500);
            
            IsLoading = false;
            StateHasChanged();
            
            _pollingTimer = new System.Timers.Timer(PollingInterval);
            _pollingTimer.Elapsed += async (_, __) => await PollAdminData();
            _pollingTimer.AutoReset = true;
            _pollingTimer.Start();
        }
        
        private async Task PollAdminData()
        {
            if (_isPolling) return;
            _isPolling = true;

            try
            {
                Users = await AdminService.GetAllUsers();
            }
            finally
            {
                _isPolling = false;
                await InvokeAsync(StateHasChanged);
            }
        }
        public void Dispose()
        {
            _pollingTimer?.Stop();
            _pollingTimer?.Dispose();
        }

        private void OpenRegisterDialog()
        {
            ShowRegisterDialog = true;
            StateHasChanged();
        }
        
        private void CloseRegisterDialog()
        {
            ShowRegisterDialog = false;
        }

        private void OpenEditUserDialog(UserDto user)
        {
            SelectedUser = new EditModel
            {
                Id = user.Id,
                Email = user.Email,
                Username = user.UserName,
                Role = user.Role
            };
            ShowEditUserDialog = true;
            StateHasChanged();
        }
        private void CloseEditUserDialog()
        {
            ShowEditUserDialog = false;
            SelectedUser = null;
        }

        private void OpenNewPerformanceDialog()
        {
            ShowNewPerformanceDialog = true;
            StateHasChanged();
        }
        private void CloseNewPerformanceDialog()
        {
            ShowNewPerformanceDialog = false;
        }
        private void OpenEditPerformanceDialog(DjSets performance)
        {
            SelectedPerformance = performance;
            ShowEditPerformanceDialog = true;
            StateHasChanged();
        }

        private void CloseEditPerformanceDialog()
        {
            ShowEditPerformanceDialog = false;
            SelectedPerformance = null;
        }
        private void OpenCreateClubDialog()
        {
            SelectedClub = new Clubs
            {
                ClubName = string.Empty,
                Location = string.Empty,
                Floor = 0
            };
            ShowCreateClubDialog = true;
        }

        private void OpenEditClubDialog(Clubs club)
        {
            SelectedClub = new Clubs
            {
                Id = club.Id,
                ClubName = club.ClubName,
                Location = club.Location,
                Floor = club.Floor
            };
            ShowEditClubDialog = true;
        }
        
        private async Task OnUserEdited()
        {
            Users = await AdminService.GetAllUsers();
            CloseEditUserDialog();
            StateHasChanged();
        }
        
        private async Task OnPerformanceCreated()
        {
            DjSets = await AdminService.GetAllDjSets();
            CloseNewPerformanceDialog();
            StateHasChanged();
        }
        private async Task ReloadPerformances()
        {
            DjSets = await AdminService.GetAllDjSets();
            ShowEditPerformanceDialog = false;
            StateHasChanged();
        }

        private async Task DeleteUser(string userId)
        {
            var confirmed = await Js.InvokeAsync<bool>(
                "confirm",
                "Are you sure you want to delete this user?"
            );

            if (!confirmed)
                return;

            var success = await AdminService.DeleteUser(userId);

            if (success)
            {
                Users = await AdminService.GetAllUsers();
                StateHasChanged();
            }
        }

        private async Task DeletePerformance(int djSetId)
        {
            var confirmed = await Js.InvokeAsync<bool>(
                "confirm",
                "Are you sure you want to delete this performance?"
            );

            if (!confirmed)
                return;

            var success = await AdminService.DeletePerformance(djSetId);

            if (success)
            {
                DjSets = await AdminService.GetAllDjSets();
                StateHasChanged();
            }
        }
        
        private async Task DeleteClub(int clubId)
        {
            var confirmed = await Js.InvokeAsync<bool>(
                "confirm",
                "Are you sure you want to delete this club?"
            );

            if (!confirmed)
                return;

            var success = await AdminService.DeleteClub(clubId);

            if (success)
            {
                Clubs = await AdminService.GetAllClubs();
                StateHasChanged();
            }
        }
        
        private async Task GetAllSongs(string clubId)
        {
            DiagramData = await AdminService.GetAllSongs(clubId);
        }
        
        private async Task OnDiagramChanged(ChangeEventArgs e)
        {
            var selectedValue = e.Value?.ToString();

            if (string.IsNullOrWhiteSpace(selectedValue))
            {
                DiagramData = null;             
                _selectedClubName = null;
                _diagramKey = Guid.NewGuid().ToString();
                StateHasChanged();
                return;
            }

            if (selectedValue.Equals("all", StringComparison.OrdinalIgnoreCase))
            {
                _selectedClubName = "All Clubs";
            }
            else
            {
                var clubId = int.Parse(selectedValue);
                var club = Clubs?.FirstOrDefault(c => c.Id == clubId);
                _selectedClubName = club != null
                    ? $"{club.Location} - {club.ClubName}"
                    : "Unknown Club";
            }

            DiagramData = await AdminService.GetAllSongs(selectedValue);

            _diagramKey = Guid.NewGuid().ToString();
        }
        
        private void ChangeUserPage(int page)
        {
            if (page < 1 || page > TotalUserPages) return;
            CurrentUserPage = page;
            StateHasChanged();
        }
        
        private void ChangePerformancePage(int page)
        {
            if (page < 1 || page > TotalPerformancePages) return;
            CurrentPerformancePage = page;
            StateHasChanged();
        }
        private void ChangeClubPage(int page)
        {
            if (page < 1 || page > TotalClubPages) return;
            CurrentClubPage = page;
            StateHasChanged();
        }
        
        private async Task OnClubSaved()
        {
            Clubs = await AdminService.GetAllClubs();
            ShowCreateClubDialog = false;
            ShowEditClubDialog = false;
            SelectedClub = null;
            StateHasChanged();
        }

    }
}