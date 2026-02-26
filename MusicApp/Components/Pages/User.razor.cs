using DatabaseLayer.DbTables;
using Microsoft.AspNetCore.Components;
using MusicApp.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using MusicApp.Models;

namespace MusicApp.Components.Pages
{
    public partial class User : ComponentBase, IDisposable
    {
        [Inject] private UserService UserService { get; set; } = default!;
        [Inject] private AuthenticationStateProvider AuthStateProvider { get; set; } = default!;
        [Inject] private SpotifyService SpotifyService { get; set; } = default!;
        [Inject] private IJSRuntime Js { get; set; } = default!;

        private IEnumerable<Songs>? UserCurrentClubSongs { get; set; }
        private IEnumerable<Songs>? UserPastClubSongs { get; set; }
        private IEnumerable<Clubs>? Clubs { get; set; }
        private int? SelectedClub { get; set; }
        private bool ShowRequestDialog { get; set; } = false;
        private SongRequestModel NewSong { get; set; } = new();
        private string _searchQuery = string.Empty;
        private List<(string Title, string Artist)> _searchResults = new();
        private bool _showResults = false;
        private DjSets? CurrentDjSet { get; set; }
        private string? CurrentDjName { get; set; }
        private string? CurrentClubName { get; set; }
        private string? ErrorMessage { get; set; }
        private bool IsLoading { get; set; } = true;
        private bool IsClubLoading { get; set; } = true;
        private bool _firstRender = true;
        private System.Timers.Timer? _pollingTimer;
        private const int PollingInterval = 5000;
        private bool _isPolling;
        private System.Timers.Timer? _typingTimer;
        private int CurrentSongPage { get; set; } = 1;
        private int SongPageSize { get; set; } = 5;
        
        private IEnumerable<Songs> PagedCurrentSongs =>
            UserCurrentClubSongs == null
                ? Enumerable.Empty<Songs>()
                : UserCurrentClubSongs
                    .OrderByDescending(x => x.RequestedTime)
                    .Skip((CurrentSongPage - 1) * SongPageSize)
                    .Take(SongPageSize);
        
        private int TotalSongPages =>
            UserCurrentClubSongs == null ? 1 : (int)Math.Ceiling((double)UserCurrentClubSongs.Count() / SongPageSize);
        
        private int CurrentPastSongPage { get; set; } = 1;
        private int PastSongPageSize { get; set; } = 10;

        private IEnumerable<Songs> PagedPastSongs =>
            UserPastClubSongs == null
                ? Enumerable.Empty<Songs>()
                : UserPastClubSongs.Skip((CurrentPastSongPage - 1) * PastSongPageSize)
                    .Take(PastSongPageSize);

        private int TotalPastSongPages =>
            UserPastClubSongs == null ? 1 : (int)Math.Ceiling((double)UserPastClubSongs.Count() / PastSongPageSize);
        

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!_firstRender)
                return;

            _firstRender = false;

            IsLoading = true;

            Clubs = await UserService.GetAllClubs();
            await Task.Delay(500);
            
            var storedClubId = await Js.InvokeAsync<string>(
                "localStorage.getItem",
                "selectedClubId"
            );

            if (int.TryParse(storedClubId, out var clubId)
                && Clubs.Any(c => c.Id == clubId))
            {
                SelectedClub = clubId;
                
                await OnClubChanged(new ChangeEventArgs
                {
                    Value = clubId.ToString()
                });
                
            }
            
            IsLoading = false;
            StateHasChanged();
            
            _pollingTimer = new System.Timers.Timer(PollingInterval);
            _pollingTimer.Elapsed += async (_, __) => await PollUserSongs();
            _pollingTimer.AutoReset = true;
            _pollingTimer.Start();
        }
        
        

        private async Task PollUserSongs()
        {
            if (_isPolling || SelectedClub == null) return;
            _isPolling = true;

            try
            {
                var authState = await AuthStateProvider.GetAuthenticationStateAsync();
                var user = authState.User;
                var userId = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                if (!string.IsNullOrWhiteSpace(userId))
                {
                    var (songs, error) = await UserService.GetUserCurrentClubSongs(userId, SelectedClub);

                    if (string.IsNullOrEmpty(error))
                    {
                        UserCurrentClubSongs = songs;
                    }
                }
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
        
        private async Task OnClubChanged(ChangeEventArgs e)
        {
            if (!int.TryParse(e.Value?.ToString(), out int clubId))
                return;

            SelectedClub = clubId;
            await Js.InvokeVoidAsync("localStorage.setItem", "selectedClubId", clubId.ToString());
            IsClubLoading = true;
            StateHasChanged();
            await Task.Delay(500);

            var authState = await AuthStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;
            var userId = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(userId))
            {
                IsClubLoading = false;
                return;
            }

            var (songs, error) = await UserService.GetUserCurrentClubSongs(userId, SelectedClub);

            if (!string.IsNullOrEmpty(error))
            {
                ErrorMessage = error;
                UserCurrentClubSongs = Enumerable.Empty<Songs>();
                CurrentDjSet = null;
                CurrentDjName = null;
                CurrentClubName = null;
            }
            else
            {
                ErrorMessage = null;
                UserCurrentClubSongs = songs;

                UserPastClubSongs = await UserService.GetUserPastClubSongs(userId, SelectedClub);
                
                CurrentDjSet = UserCurrentClubSongs.FirstOrDefault()?.DjSets;
                
                if (CurrentDjSet == null)
                {
                    CurrentDjSet = await UserService.GetCurrentDjSetForClub(SelectedClub.Value);
                }

                if (CurrentDjSet != null)
                {
                    CurrentDjName = await UserService.GetCurrentDj(CurrentDjSet.DjId.ToString());
                    CurrentClubName = await UserService.GetCurrentClubName(CurrentDjSet.ClubId.ToString());
                }
                else
                {
                    CurrentDjName = null;
                    CurrentClubName = null;
                }
            }
            IsClubLoading = false;
            StateHasChanged();
        }

        
        private void OpenRequestDialog()
        {
            NewSong = new SongRequestModel();
            _searchQuery = string.Empty;
            _searchResults.Clear();
            _showResults = false;

            ShowRequestDialog = true;
            StateHasChanged();
        }
        
        private void CloseRequestDialog()
        {
            ShowRequestDialog = false;
            NewSong = new SongRequestModel();
            _searchQuery = string.Empty;
            _searchResults.Clear();
            _showResults = false;

            StateHasChanged();
        }
        
        private async Task SubmitRequest()
        {
            if (string.IsNullOrWhiteSpace(NewSong.Author) || string.IsNullOrWhiteSpace(NewSong.Title))
                return;

            var authState = await AuthStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;
            var userId = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (!string.IsNullOrWhiteSpace(userId) && SelectedClub != null)
            {
                await UserService.RequestSong(userId, SelectedClub.Value, NewSong.Author, NewSong.Title);
                var (songs, error) = await UserService.GetUserCurrentClubSongs(userId, SelectedClub);

                if (!string.IsNullOrEmpty(error))
                {
                    ErrorMessage = error;
                    UserCurrentClubSongs = Enumerable.Empty<Songs>();
                    CurrentDjSet = null;
                    CurrentDjName = null;
                    CurrentClubName = null;
                }
                else
                {
                    ErrorMessage = null;
                    UserCurrentClubSongs = songs;
                    CurrentDjSet = UserCurrentClubSongs.FirstOrDefault()?.DjSets;

                    if (CurrentDjSet != null)
                    {
                        CurrentDjName = await UserService.GetCurrentDj(CurrentDjSet.DjId.ToString());
                        CurrentClubName = await UserService.GetCurrentClubName(CurrentDjSet.ClubId.ToString());
                    }
                }
            }

            ShowRequestDialog = false;
            StateHasChanged();
        }

        
        private async Task SearchSongs()
        {
            if (string.IsNullOrWhiteSpace(_searchQuery))
            {
                _searchResults.Clear();
                _showResults = false;
                return;
            }

            _searchResults = await SpotifyService.SearchTracksAsync(_searchQuery);
            _showResults = true;
        }
        private void SelectSong((string Title, string Artist) song)
        {
            NewSong.Author = song.Artist;
            NewSong.Title = song.Title;
            _searchQuery = $"{song.Artist} - {song.Title}";
            _showResults = false;
        }
        private void StartDebounceTimer()
        {
            _typingTimer?.Stop();
            _typingTimer = new System.Timers.Timer(400);
            _typingTimer.Elapsed += async (_, __) =>
            {
                _typingTimer.Stop();
                await InvokeAsync(async () => await SearchSongs());
            };
            _typingTimer.AutoReset = false;
            _typingTimer.Start();
        }
        private Task HandleInputChange(ChangeEventArgs e)
        {
            _searchQuery = e.Value?.ToString() ?? string.Empty;
        
            if (_searchQuery.Length > 5)
            {
                StartDebounceTimer();
            }
            else
            {
                _searchResults.Clear();
                _showResults = false;
            }

            return Task.CompletedTask;
        }

        private async Task RequestPastSong(Songs pastSong)
        {
            var authState = await AuthStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;
            var userId = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(userId) || SelectedClub == null)
                return;
            
            await UserService.RequestSong(userId, SelectedClub.Value, pastSong.Author!, pastSong.Title);
            
            var (songs, error) = await UserService.GetUserCurrentClubSongs(userId, SelectedClub);

            if (!string.IsNullOrEmpty(error))
            {
                ErrorMessage = error;
                UserCurrentClubSongs = Enumerable.Empty<Songs>();
                CurrentDjSet = null;
                CurrentDjName = null;
                CurrentClubName = null;
            }
            else
            {
                ErrorMessage = null;
                UserCurrentClubSongs = songs;
                CurrentDjSet = UserCurrentClubSongs.FirstOrDefault()?.DjSets;

                if (CurrentDjSet != null)
                {
                    CurrentDjName = await UserService.GetCurrentDj(CurrentDjSet.DjId.ToString());
                    CurrentClubName = await UserService.GetCurrentClubName(CurrentDjSet.ClubId.ToString());
                }
            }

            StateHasChanged();
        }
        
        private void ChangePastSongPage(int page)
        {
            if (page < 1 || page > TotalPastSongPages) return;
            CurrentPastSongPage = page;
            StateHasChanged();
        }
        private void ChangeSongPage(int page)
        {
            if (page < 1 || page > TotalSongPages) return;
            CurrentSongPage = page;
            StateHasChanged();
        }
        private string GetStatusColor(string? status)
        {
            return status?.ToLower() switch
            {
                "accepted" => "green",
                "rejected" => "red",
                "pending" => "orange",
                _ => "white"
            };
        }

    }
}