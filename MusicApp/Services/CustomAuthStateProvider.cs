using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;

namespace MusicApp.Services
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        
        private ClaimsPrincipal _currentUser;
        private bool _initialized;

        public CustomAuthStateProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
            _initialized = false;
        }

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            if (!_initialized)
            {
                var httpUser = _httpContextAccessor.HttpContext?.User;
                if (httpUser != null && httpUser.Identity?.IsAuthenticated == true)
                {
                    _currentUser = httpUser;
                }

                _initialized = true;
            }

            return Task.FromResult(new AuthenticationState(_currentUser));
        }
        
        public void ForceSignOut()
        {
            _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
            
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));
        }
    }
}
