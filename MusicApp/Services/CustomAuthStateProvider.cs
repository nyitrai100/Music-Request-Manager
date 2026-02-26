// using System.Security.Claims;
// using Microsoft.AspNetCore.Components.Authorization;
//
// namespace MusicApp.Services
// {
//     public class CustomAuthStateProvider : AuthenticationStateProvider
//     {
//         private readonly IHttpContextAccessor _httpContextAccessor;
//
//         public CustomAuthStateProvider(IHttpContextAccessor httpContextAccessor)
//         {
//             _httpContextAccessor = httpContextAccessor;
//         }
//
//         public override Task<AuthenticationState> GetAuthenticationStateAsync()
//         {
//             var user = _httpContextAccessor.HttpContext?.User;
//             if (user == null)
//                 user = new ClaimsPrincipal(new ClaimsIdentity());
//         
//             return Task.FromResult(new AuthenticationState(user));
//         }
//         
//         public void ForceSignOut()
//         {
//             var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
//             NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(anonymousUser)));
//         }
//     }
// }

// using System.Security.Claims;
// using Microsoft.AspNetCore.Components.Authorization;
// using Microsoft.AspNetCore.Http;
//
// namespace MusicApp.Services
// {
//     public class CustomAuthStateProvider : AuthenticationStateProvider
//     {
//         private readonly IHttpContextAccessor _httpContextAccessor;
//         private ClaimsPrincipal? _currentUser;
//
//         public CustomAuthStateProvider(IHttpContextAccessor httpContextAccessor)
//         {
//             _httpContextAccessor = httpContextAccessor;
//         }
//
//         public override Task<AuthenticationState> GetAuthenticationStateAsync()
//         {
//             if (_currentUser == null)
//             {
//                 _currentUser = _httpContextAccessor.HttpContext?.User;
//                 
//                 if (_currentUser == null || !_currentUser.Identity?.IsAuthenticated == true)
//                 {
//                     _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
//                 }
//             }
//
//             return Task.FromResult(new AuthenticationState(_currentUser));
//         }
//         
//         public void ForceSignOut()
//         {
//             _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
//             NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));
//         }
//     }
// }

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

        /// <summary>
        /// Call this when logging out
        /// </summary>
        public void ForceSignOut()
        {
            // ✅ Set in-memory user to anonymous
            _currentUser = new ClaimsPrincipal(new ClaimsIdentity());

            // ✅ Notify all components subscribed to auth state
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));
        }
    }
}
