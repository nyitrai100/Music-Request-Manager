using DatabaseLayer.DbTables;

namespace MusicApp.Services;

public class UserService
{
    private readonly HttpClient _http;
    public UserService(HttpClient http)
    {
        _http = http;
    }
    public async Task<(IEnumerable<Songs> Songs, string? ErrorMessage)> GetUserCurrentClubSongs(string userId, int? clubId)
    {
        var response = await _http.GetAsync($"api/user/songs?userId={userId}&clubId={clubId}");

        if (response.IsSuccessStatusCode)
        {
            var songs = await response.Content.ReadFromJsonAsync<IEnumerable<Songs>>();
            return (songs ?? Enumerable.Empty<Songs>(), null);
        }
        
        var error = await response.Content.ReadAsStringAsync();
        return (Enumerable.Empty<Songs>(), error);
    }


    public async Task<IEnumerable<Clubs>> GetAllClubs()
    {
        var result = await _http.GetFromJsonAsync<IEnumerable<Clubs>>($"api/user/clubs");
        return result ?? Enumerable.Empty<Clubs>();
    }

    public async Task RequestSong(string userId, int clubId, string author, string title)
    {
        var djSet = await _http.GetFromJsonAsync<DjSets>($"api/user/djSets?clubId={clubId}");
        if(djSet == null)
            return;
        
        var newSong = new { UserId = userId, ClubId = clubId, Author = author, Title = title, RequestedTime = DateTime.Now, StatusId = 3, DjSetId= djSet.Id };
        var response = await _http.PostAsJsonAsync("api/user/songs", newSong);
        response.EnsureSuccessStatusCode();
    }
    public async Task<string?> GetCurrentDj(string djId)
    {
        return await _http.GetStringAsync($"api/user/currentDj?djId={djId}");
    }

    public async Task<string?> GetCurrentClubName(string clubId)
    {
        return await _http.GetStringAsync($"api/user/currentClub?clubId={clubId}");
    }

    public async Task<DjSets?> GetCurrentDjSetForClub(int clubId)
    {
        var response = await _http.GetAsync($"api/user/currentDjSet?clubId={clubId}");
        if (response.IsSuccessStatusCode)
            return await response.Content.ReadFromJsonAsync<DjSets>();
        return null;
    }

    public async Task<IEnumerable<Songs>?> GetUserPastClubSongs(string userId, int? clubId)
    {
        var response = await _http.GetAsync($"api/user/pastSongs?userId={userId}&clubId={clubId}");

        if (response.IsSuccessStatusCode)
        {
            var songs = await response.Content.ReadFromJsonAsync<IEnumerable<Songs>>();
            return songs ?? Enumerable.Empty<Songs>();
        }

        return null;
    }
}