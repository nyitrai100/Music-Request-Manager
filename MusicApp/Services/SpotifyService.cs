using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace MusicApp.Services;

public class SpotifyService
{
    private readonly HttpClient _http = new();
    private readonly string _clientId;
    private readonly string _clientSecret;

    public SpotifyService(IConfiguration config)
    {
        _clientId = config["Spotify:ClientId"]!;
        _clientSecret = config["Spotify:ClientSecret"]!;
    }

    private async Task<string> GetAccessTokenAsync()
    {
        var authBytes = Encoding.UTF8.GetBytes($"{_clientId}:{_clientSecret}");

        var request = new HttpRequestMessage(HttpMethod.Post, "https://accounts.spotify.com/api/token");
        request.Headers.Authorization =
            new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authBytes));

        request.Content = new StringContent(
            "grant_type=client_credentials",
            Encoding.UTF8,
            "application/x-www-form-urlencoded"
        );

        var response = await _http.SendAsync(request);
        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync();
        var json = await JsonDocument.ParseAsync(stream);

        return json.RootElement.GetProperty("access_token").GetString()!;
    }

    public async Task<List<(string Title, string Artist)>> SearchTracksAsync(string query)
    {
        var token = await GetAccessTokenAsync();

        var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"https://api.spotify.com/v1/search?q={Uri.EscapeDataString(query)}&type=track&limit=10"
        );

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