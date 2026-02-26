using DatabaseLayer.DbTables;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MusicApp.Services;
using Blazorise.Charts;

namespace MusicApp.Components.Pages
{
    public partial class Dj : ComponentBase, IDisposable
    {
        [Inject] private DjService DjService { get; set; } = default!;
        [Inject] private AuthenticationStateProvider AuthStateProvider { get; set; } = default!;
        private IEnumerable<Songs>? CurrentSongs { get; set; }
        private IEnumerable<Songs>? PastSongs { get; set; }
        private IEnumerable<Songs>? FutureSongs { get; set; }
        private IEnumerable<Songs>? DiagramData { get; set; }
        private Clubs? CurrentClub { get; set; }
        private string? CurrentClubErrorMessage { get; set; }
        private string? PastClubErrorMessage { get; set; }
        private string? FutureClubErrorMessage { get; set; }
        private string? DiagramDataErrorMessage { get; set; }
        private DateTime? CurrentPerformanceTimeStarts { get; set; }
        private DateTime? CurrentPerformanceTimeEnds { get; set; }
        private DateTime? FuturePerformanceTimeStarts { get; set; }
        private DateTime? FuturePerformanceTimeEnds { get; set; }
        private const string Current = "current";
        private const string Past = "past";
        private const string Future = "future";
        private int? SelectedClub { get; set; }
        private IEnumerable<Clubs>? Clubs { get; set; }
        private Clubs? PastClub { get; set; }
        private string? FutureClub { get; set; }
        private string? FutureClubLocation { get; set; }
        private bool IsLoading { get; set; } = true;
        private bool IsClubLoading { get; set; } = true;
        private bool _firstRender = true;
        private System.Timers.Timer? _pollingTimer;
        private const int PollingInterval = 5000; 
        private bool _isPolling;
        private int AcceptedCurrentPage { get; set; } = 1;
        private int AcceptedPageSize { get; set; } = 5;
        private IEnumerable<Songs> PagedAcceptedSongs =>
            CurrentSongs?.Where(s => s.StatusId == 1)
                         .Skip((AcceptedCurrentPage - 1) * AcceptedPageSize)
                         .Take(AcceptedPageSize) ?? Enumerable.Empty<Songs>();
        private int AcceptedTotalPages =>
            CurrentSongs == null ? 1 : (int)Math.Ceiling(CurrentSongs.Count(s => s.StatusId == 1) / (double)AcceptedPageSize);
        private int RejectedCurrentPage { get; set; } = 1;
        private int RejectedPageSize { get; set; } = 5;
        private IEnumerable<Songs> PagedRejectedSongs =>
            CurrentSongs?.Where(s => s.StatusId == 2)
                         .Skip((RejectedCurrentPage - 1) * RejectedPageSize)
                         .Take(RejectedPageSize) ?? Enumerable.Empty<Songs>();
        private int RejectedTotalPages =>
            CurrentSongs == null ? 1 : (int)Math.Ceiling(CurrentSongs.Count(s => s.StatusId == 2) / (double)RejectedPageSize);
        private int PendingCurrentPage { get; set; } = 1;
        private int PendingPageSize { get; set; } = 5;
        private IEnumerable<Songs> PagedPendingSongs =>
            CurrentSongs?.Where(s => s.StatusId == 3)
                         .Skip((PendingCurrentPage - 1) * PendingPageSize)
                         .Take(PendingPageSize) ?? Enumerable.Empty<Songs>();
        private int PendingTotalPages =>
            CurrentSongs == null ? 1 : (int)Math.Ceiling(CurrentSongs.Count(s => s.StatusId == 3) / (double)PendingPageSize);
        private int PastCurrentPage { get; set; } = 1;
        private int PastPageSize { get; set; } = 5;
        private IEnumerable<Songs> PagedPastSongs =>
            PastSongs?.Skip((PastCurrentPage - 1) * PastPageSize)
                      .Take(PastPageSize) ?? Enumerable.Empty<Songs>();
        private int PastTotalPages =>
            PastSongs == null ? 1 : (int)Math.Ceiling(PastSongs.Count() / (double)PastPageSize);
     
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!_firstRender)
                return;

            _firstRender = false;
    
            IsLoading = true;
            StateHasChanged();

            await LoadCurrentSongs();
            await LoadClubDropDown();
            await LoadFutureClub();

            await Task.Delay(500);
            IsLoading = false;

            StateHasChanged();
            
            _pollingTimer = new System.Timers.Timer(PollingInterval);
            _pollingTimer.Elapsed += async (_, __) => await PollCurrentSongs();
            _pollingTimer.AutoReset = true;
            _pollingTimer.Start();
            
        }
        
        
        private async Task PollCurrentSongs()
        {
            if (_isPolling) return;
            _isPolling = true;

            try
            {
                var djId = await GetDjId();
                if (string.IsNullOrEmpty(djId))
                    return;

                var (songs, error) = await DjService.GetDjSongs(djId, Current);
                if (string.IsNullOrEmpty(error))
                {
                    CurrentSongs = songs.ToList();
                }
                
                await LoadFutureClub();
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
    

        private async Task<string> GetDjId()
        {
            var authState = await AuthStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;
            var djId = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            return djId ?? string.Empty;
        }
        
        private async Task LoadCurrentSongs()
        {
            var djId = await GetDjId();
            if (string.IsNullOrEmpty(djId))
                return;

            var (songs, error) = await DjService.GetDjSongs(djId, Current);

            if (!string.IsNullOrEmpty(error))
            {
                CurrentClubErrorMessage = error;
                CurrentSongs = Enumerable.Empty<Songs>();
            }
            else
            {
                CurrentClubErrorMessage = null;
                songs = songs.ToList();
                CurrentSongs = songs;
                CurrentClub = songs.FirstOrDefault()?.Club;
                CurrentPerformanceTimeStarts = songs.FirstOrDefault()?.DjSets?.PerformanceTimeStarts;
                CurrentPerformanceTimeEnds = songs.FirstOrDefault()?.DjSets?.PerformanceTimeEnds;
            }
        }

        private async Task LoadClubDropDown()
        {
           var allClubs = await DjService.GetAllClubs();
           Clubs = allClubs;
        }
        
        private async Task UpdateStatus(Songs song, int statusId)
        {
             await DjService.UpdateStatus(song, statusId);
             await LoadCurrentSongs();
        }
        
        private async Task LoadPastSongs(int? selectedId)
        {
            var djId = await GetDjId();
            if (string.IsNullOrEmpty(djId))
                return;

            var (songs, error) = await DjService.GetDjSongs(djId, Past);

            if (!string.IsNullOrEmpty(error))
            {
                PastClubErrorMessage = error;
                PastSongs = Enumerable.Empty<Songs>();
            }
            else
            {
                PastClubErrorMessage = null;
                songs = songs.Where(x => x.ClubId == selectedId).ToList();
                PastSongs = songs;
                PastClub = songs.FirstOrDefault()?.Club;
            }
        }
        
        private async Task OnClubChanged(ChangeEventArgs e)
        {
            if (!int.TryParse(e.Value?.ToString(), out int clubId))
                return;

            SelectedClub = clubId;
            IsClubLoading = true;
            await Task.Delay(500);
            
            if (SelectedClub == null || SelectedClub == 0)
            {
                PastSongs = Enumerable.Empty<Songs>();
                PastClubErrorMessage = "Please select a valid club.";
                IsClubLoading = false;
                return;
            }

            await LoadPastSongs(SelectedClub);
            await LoadDiagramData(SelectedClub);
            await Task.Delay(50);
            IsClubLoading = false;
            StateHasChanged();
            
        }
        
        
        private async Task LoadFutureClub()
        {
            var djId = await GetDjId();
            if (string.IsNullOrEmpty(djId))
                return;

            var (songs, error) = await DjService.GetDjSongs(djId, Future);

            if (!string.IsNullOrEmpty(error))
            {
                FutureClubErrorMessage = error;
                FutureSongs = Enumerable.Empty<Songs>();
            }
            else
            {
                FutureClubErrorMessage = null;
                songs = songs.ToList();
                FutureSongs = songs;
                FutureClub = songs.FirstOrDefault()?.Club?.ClubName;
                FutureClubLocation = songs.FirstOrDefault()?.Club?.Location;
                FuturePerformanceTimeStarts = songs.FirstOrDefault()?.DjSets?.PerformanceTimeStarts;
                FuturePerformanceTimeEnds = songs.FirstOrDefault()?.DjSets?.PerformanceTimeEnds;
            }
        }
        
        
        private async Task LoadDiagramData(int? selectedCLubId)
        {
            if (selectedCLubId == null)
                return;
            

            var djId = await GetDjId();
            if (string.IsNullOrEmpty(djId))
                return;

            var (songs, error) = await DjService.GetDjSongs(djId, Past);

            if (!string.IsNullOrEmpty(error))
            {
                DiagramDataErrorMessage = error;
                DiagramData = Enumerable.Empty<Songs>();
            }
            else
            {
                DiagramDataErrorMessage = null;
                songs = songs.Where(x => x.ClubId == selectedCLubId).ToList();
                DiagramData = songs;
                StateHasChanged();
            }
        }
        
        private void ChangePastPage(int page)
        {
            if (page < 1 || page > PastTotalPages) return;
            PastCurrentPage = page;
            StateHasChanged();
        }
        private void ChangeAcceptedPage(int page)
        {
            if (page < 1 || page > AcceptedTotalPages) return;
            AcceptedCurrentPage = page;
            StateHasChanged();
        }
        private void ChangePendingPage(int page)
        {
            if (page < 1 || page > PendingTotalPages) return;
            PendingCurrentPage = page;
            StateHasChanged();
        }
        private void ChangeRejectedPage(int page)
        {
            if (page < 1 || page > RejectedTotalPages) return;
            RejectedCurrentPage = page;
            StateHasChanged();
        }
    }
}