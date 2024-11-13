using System.Security.Claims;
using DTOs;

namespace BlazorApp.Services
{
    public class SimpleAuthService
    {
        public UserDTO? CurrentUser { get; private set; }

        public bool IsAuthenticated => CurrentUser != null;

        public List<Claim> GetUserClaims()
        {
            if (CurrentUser == null)
                return new List<Claim>();

            return new List<Claim>
            {
                new Claim(ClaimTypes.Name, CurrentUser.Username),
                new Claim(ClaimTypes.Email, CurrentUser.Email ?? ""),
                new Claim("Name", CurrentUser.Name ?? "")
            };
        }

        public void SetCurrentUser(UserDTO user)
        {
            CurrentUser = user;
        }

        public void Logout()
        {
            CurrentUser = null;
        }
    }
}