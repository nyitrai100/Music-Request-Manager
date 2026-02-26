using DatabaseLayer.DbTables;
using MusicApp.Models;
using DbDj = DatabaseLayer.DbTables.Dj;

namespace MusicApp.Services;

public class AdminService
{
    private readonly HttpClient _http;
    public AdminService(HttpClient http)
    {
        _http = http;
    }
    
    public async Task<List<UserDto>> GetAllUsers()
    {
        var result = await _http.GetFromJsonAsync<List<UserDto>>("api/Admin/allUsers");
        return result ?? new List<UserDto>();
    }
    public async Task<List<DjSets>> GetAllDjSets()
    {
        var result = await _http.GetFromJsonAsync<List<DjSets>>($"api/Admin/djSets");
        return result ?? [];
    }
    
    public async Task<IEnumerable<Songs>> GetAllSongs(string selectedClub)
    {
        var result = await _http.GetFromJsonAsync<List<Songs>>($"api/Admin/allSongs?clubId={selectedClub}");
        return result ?? [];
    }
    public async Task<(bool Success, string? Error)> EditUser(EditModel model)
    {
        var formData = new MultipartFormDataContent
        {
            { new StringContent(model.Id), "Id" },
            { new StringContent(model.Email), "Email" },
            { new StringContent(model.Username), "Username" },
            { new StringContent(model.Role), "Role" }
        };

        if (!string.IsNullOrWhiteSpace(model.Password))
        {
            formData.Add(new StringContent(model.Password), "Password");
        }

        var response = await _http.PutAsync("api/auth/edit", formData);

        if (response.IsSuccessStatusCode)
            return (true, null);

        var error = await response.Content.ReadAsStringAsync();
        return (false, error);
    }
    
    public async Task<(bool Success, string? Error)> RegisterUser(RegisterModel model)
    {
        var formData = new MultipartFormDataContent
        {
            { new StringContent(model.Email), "Email" },
            { new StringContent(model.Username), "Username" },
            { new StringContent(model.Password), "Password" },
            { new StringContent(model.ConfirmPassword), "ConfirmPassword" },
            { new StringContent(model.Role), "Role" }
        };

        var response = await _http.PostAsync("api/auth/register", formData);

        if (response.IsSuccessStatusCode)
            return (true, null);

        var error = await response.Content.ReadAsStringAsync();
        return (false, error);
    }
    
    public async Task<List<DbDj>> GetAllDjs()
    {
        var result = await _http.GetFromJsonAsync<List<DbDj>>("api/admin/allDj");
        return result ?? [];
    }

    public async Task<List<Clubs>> GetAllClubs()
    {
        var result = await _http.GetFromJsonAsync<List<Clubs>>("api/admin/allClub");
        return result ?? [];
    }

    public async Task<(bool Success, string? Error)> CreatePerformance(CreatePerformanceModel model)
    {
        var response = await _http.PostAsJsonAsync("api/admin/createPerformance", model);

        if (response.IsSuccessStatusCode)
            return (true, null);

        var error = await response.Content.ReadAsStringAsync();
        return (false, error);
    }
    
    public async Task<(bool Success, string? Error)> EditPerformance(EditPerformanceModel model)
    {
        var response = await _http.PutAsJsonAsync("api/admin/editPerformance", model);

        if (response.IsSuccessStatusCode)
            return (true, null);

        var error = await response.Content.ReadAsStringAsync();
        return (false, error);
    }
    
    public async Task<bool> DeleteUser(string userId)
    {
        var response = await _http.DeleteAsync($"api/auth/delete/{userId}");
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeletePerformance(int djSetId)
    {
        var response = await _http.DeleteAsync($"api/admin/delete/{djSetId}");
        return response.IsSuccessStatusCode;
    }
    
    public async Task<(bool Success, string? Error)> CreateClub(Clubs model)
    {
        var response = await _http.PostAsJsonAsync("api/admin/createClub", model);

        if (response.IsSuccessStatusCode)
            return (true, null);

        var error = await response.Content.ReadAsStringAsync();
        return (false, error);
    }

    public async Task<(bool Success, string? Error)> EditClub(Clubs model)
    {
        var response = await _http.PutAsJsonAsync("api/admin/editClub", model);

        if (response.IsSuccessStatusCode)
            return (true, null);

        var error = await response.Content.ReadAsStringAsync();
        return (false, error);
    }

    public async Task<bool> DeleteClub(int clubId)
    {
        var response = await _http.DeleteAsync($"api/admin/deleteClub/{clubId}");
        return response.IsSuccessStatusCode;
    }


}