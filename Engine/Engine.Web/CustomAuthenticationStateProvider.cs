using Caspian.Common;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace Caspian.Engine.Web
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        string UserId;
        DateTime? dateTime;
        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            ClaimsPrincipal user = null;
            if (UserId.HasValue() && (DateTime.Now - dateTime.Value).TotalMinutes > 20)
                UserId = null;
            if (UserId.HasValue())
            {
                var identity = new ClaimsIdentity(new[]
                {
                    new Claim("UserId", UserId ?? ""),
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
            dateTime = DateTime.Now;
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
