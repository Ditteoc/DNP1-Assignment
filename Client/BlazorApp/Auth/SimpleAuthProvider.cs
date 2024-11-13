using System.Security.Claims;
using System.Text.Json;
using DTOs;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using BlazorApp.Services;

namespace BlazorApp.Auth
{
    public class SimpleAuthProvider : AuthenticationStateProvider
    {
        private readonly HttpClient _httpClient;
        private readonly IJSRuntime _jsRuntime;
        private readonly SimpleAuthService _authService;
        private bool _isInitialized;

        public SimpleAuthProvider(HttpClient httpClient, IJSRuntime jsRuntime, SimpleAuthService authService)
        {
            _httpClient = httpClient;
            _jsRuntime = jsRuntime;
            _authService = authService;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            // Vent med at udføre JavaScript interop, indtil vi er sikre på, at det er tilladt
            if (!_isInitialized)
            {
                await LoadUserFromSessionAsync();
                _isInitialized = true;
            }

            var identity = _authService.IsAuthenticated 
                ? new ClaimsIdentity(_authService.GetUserClaims(), "apiauth") 
                : new ClaimsIdentity();
            return new AuthenticationState(new ClaimsPrincipal(identity));
        }

        private async Task LoadUserFromSessionAsync()
        {
            try
            {
                // Prøv at hente brugeren fra session storage
                var userAsJson = await _jsRuntime.InvokeAsync<string>("sessionStorage.getItem", "currentUser");
                if (!string.IsNullOrEmpty(userAsJson))
                {
                    var userDto = JsonSerializer.Deserialize<UserDTO>(userAsJson);
                    _authService.SetCurrentUser(userDto);
                }
            }
            catch (InvalidOperationException)
            {
                // Ignorer fejl ved interop kald under prerendering
            }
        }

        public async Task Login(string username, string password)
        {
            Console.WriteLine("Login attempt for user: " + username);
            
            var loginRequest = new LoginRequestDTO { Username = username, Password = password };
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", loginRequest);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Forkert login.");
            }

            var userDto = await response.Content.ReadFromJsonAsync<UserDTO>();
            var userAsJson = JsonSerializer.Serialize(userDto);
            await _jsRuntime.InvokeVoidAsync("sessionStorage.setItem", "currentUser", userAsJson);

            _authService.SetCurrentUser(userDto);
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public async Task Logout()
        {
            await _jsRuntime.InvokeVoidAsync("sessionStorage.removeItem", "currentUser");
            _authService.Logout();
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()))));
        }
    }
}
