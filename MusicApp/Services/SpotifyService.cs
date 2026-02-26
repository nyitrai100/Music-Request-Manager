using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace MusicApp.Services;

public class SpotifyService
{
    private readonly HttpClient _http = new();
    private readonly string _clientId;
    private readonly string _clientSecret;
    private string? _accessToken;

    public SpotifyService(IConfiguration config)
    {
        _clientId = config["Spotify:ClientId"]!;
        _clientSecret = config["Spotify:ClientSecret"]!;
    }

    private async Task<string> GetAccessTokenAsync()
    {
        if (!string.IsNullOrEmpty(_accessToken))
            return _accessToken;

        var authBytes = Encoding.UTF8.GetBytes($"{_clientId}:{_clientSecret}");
        _http.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authBytes));

        var body = new StringContent("grant_type=client_credentials",
            Encoding.UTF8, "application/x-www-form-urlencoded");

        var response = await _http.PostAsync("https://accounts.spotify.com/api/token", body);
        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync();
        var json = await JsonDocument.ParseAsync(stream);
        _accessToken = json.RootElement.GetProperty("access_token").GetString();

        return _accessToken!;
    }
    public async Task<List<(string Title, string Artist)>> SearchTracksAsync(string query)
    {
        var token = await GetAccessTokenAsync();

        var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"https://api.spotify.com/v1/search?q={Uri.EscapeDataString(query)}&type=track&limit=10");

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _http.SendAsync(request);
        response.EnsureSuccessStatusCode();

        using var stream = await response.Content.ReadAsStreamAsync();
        var json = await JsonDocument.ParseAsync(stream);

        var results = new List<(string Title, string Artist)>();
        foreach (var item in json.RootElement
                     .GetProperty("tracks")
                     .GetProperty("items")
                     .EnumerateArray())
        {
            string title = item.GetProperty("name").GetString()!;
            string artist = item.GetProperty("artists")[0].GetProperty("name").GetString()!;
            results.Add((title, artist));
        }

        return results;
    }

}