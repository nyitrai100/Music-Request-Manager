using DatabaseLayer.DbTables;

namespace MusicApp.Services;

public class DjService
{
    private readonly HttpClient _http;
    public DjService(HttpClient http)
    {
        _http = http;
    }
    public async Task<(IEnumerable<Songs> Songs, string? ErrorMessage)> GetDjSongs(string djId, string timeScope)
    {
        var response = await _http.GetAsync($"api/dj/songs?djId={djId}&timeScope={timeScope}");

        if (response.IsSuccessStatusCode)
        {
            var songs = await response.Content.ReadFromJsonAsync<IEnumerable<Songs>>();
            songs = songs?.ToList();
            if (songs == null || !songs.Any())
                return (Enumerable.Empty<Songs>(), null);

            var clubs = await GetAllClubs();
            clubs = clubs.ToList();
            
            foreach (var song in songs)
            {
                song.Club = clubs.FirstOrDefault(c => c.Id == song.ClubId);
            }

            return (songs, null);
        }
        
        var error = await response.Content.ReadAsStringAsync();
        return (Enumerable.Empty<Songs>(), error);
    }
    
    public async Task UpdateStatus(Songs song, int statusId)
    {
        var updatedSong = new{  song.Id, song.UserId, song.ClubId, song.Author, song.Title, song.RequestedTime, song.DjSetId, StatusId = statusId };
        var response = await _http.PostAsJsonAsync($"api/dj/updateStatus", updatedSong);
        response.EnsureSuccessStatusCode();
    }

    public async Task<IEnumerable<Clubs>> GetAllClubs()
    {
        var result = await _http.GetFromJsonAsync<IEnumerable<Clubs>>($"api/user/clubs");
        return result ?? Enumerable.Empty<Clubs>();
    }
    
}