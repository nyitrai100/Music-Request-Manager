using DatabaseLayer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MusicApp.Models;
using DatabaseLayer.DbTables;
using MusicApp.Services;

namespace MusicApp.Controllers;


[ApiController]
[Microsoft.AspNetCore.Mvc.Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly AppDbContext _context;
    private CustomAuthStateProvider _customAuthStateProvider;

    public AuthController(SignInManager<IdentityUser> signInManager,
        UserManager<IdentityUser> userManager, AppDbContext context, CustomAuthStateProvider customAuthStateProvider)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _context = context;
        _customAuthStateProvider = customAuthStateProvider;
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromForm] LoginModel model)
    {
        var result = await _signInManager.PasswordSignInAsync(
            model.Username,
            model.Password,
            isPersistent: false,
            lockoutOnFailure: false
        );

        if (result.Succeeded)
        {
            return Redirect("/");
        }
        
        return Redirect($"/?error=Invalid+username+or+password");
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromForm] RegisterModel model)
    {
        if (model.Password != model.ConfirmPassword)
            return Redirect($"/?type=register&error=Passwords+do+not+match");

        var existingUser = await _userManager.FindByNameAsync(model.Username);
        if (existingUser != null)
            return Redirect($"/?type=register&error=Username+already+exists");

        var allowedRoles = new[] { "User", "DJ" };
        if (!allowedRoles.Contains(model.Role))
            return Redirect($"/?type=register&error=Invalid+role");

        var user = new IdentityUser
        {
            UserName = model.Username,
            Email = model.Email
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (!result.Succeeded)
            return BadRequest(string.Join(", ", result.Errors.Select(e => e.Description)));
        
        await _userManager.AddToRoleAsync(user, model.Role);
        
        if (model.Role == "DJ")
        {
            var dj = new Dj
            {
                UserId = user.Id
            };

            _context.Dj.Add(dj);
            await _context.SaveChangesAsync();
        }

        await _signInManager.SignInAsync(user, isPersistent: false);
        return Redirect("/");
    }
    
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        _customAuthStateProvider.ForceSignOut();

        return Redirect("/");
    }
    
    [HttpPut("edit")]
    public async Task<IActionResult> EditUser([FromForm] EditModel model)
    {
        var user = await _userManager.FindByIdAsync(model.Id);
        if (user == null)
            return NotFound("User not found");

        user.Email = model.Email;
        user.UserName = model.Username;

        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
            return BadRequest(updateResult.Errors.Select(e => e.Description));

        var currentRoles = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, currentRoles);
        await _userManager.AddToRoleAsync(user, model.Role);

        if (!string.IsNullOrEmpty(model.Password))
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var passwordResult = await _userManager.ResetPasswordAsync(
                user, token, model.Password);

            if (!passwordResult.Succeeded)
                return BadRequest(passwordResult.Errors.Select(e => e.Description));
        }

        return Ok();
    }

    
    [HttpDelete("delete/{userId}")]
    public async Task<IActionResult> DeleteUser(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
            return NotFound("User not found");

        var result = await _userManager.DeleteAsync(user);

        if (!result.Succeeded)
            return BadRequest(result.Errors.Select(e => e.Description));

        return Ok();
    }
    
}
