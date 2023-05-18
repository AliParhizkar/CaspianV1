using System.Security.Claims;
using Caspian.Common;
using Microsoft.AspNetCore.Components.Authorization;

namespace Caspian.Engine.Web
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        string UserId;
        string time;
        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            ClaimsPrincipal user = null;
            if (UserId.HasValue())
            {
                time = DateTime.Now.ToLongTimeString();
                var identity = new ClaimsIdentity(new[]
                {
                    new Claim("UserId", UserId ?? ""),
                    new Claim("DateTime", time),
                }, "Authentication type");
                user = new ClaimsPrincipal(identity);
            }
            else
                user = new ClaimsPrincipal();
            var task = Task.FromResult(new AuthenticationState(user));
            return task;
        }

        public Task<AuthenticationState> Login(string userId)
        {
            UserId = userId;
            var task = this.GetAuthenticationStateAsync();
            this.NotifyAuthenticationStateChanged(task);
            return task;
        }

        public Task<AuthenticationState> Logout()
        {
            return Login(null);
        }
    }
}
