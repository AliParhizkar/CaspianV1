using Caspian.Common.Extension;
using Caspian.Engine.Model;
using Caspian.Engine.Service;
using Caspian.Engine;
using Microsoft.AspNetCore.Components.Authorization;
using System.Linq.Dynamic.Core;
using System.Security.Claims;
using System.Collections;
using Microsoft.EntityFrameworkCore;
using Caspian.UI;

namespace Main
{
    //public class CustomAuthStateProvider: AuthenticationStateProvider
    //{
    //    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    //    {
    //        var identity = new ClaimsIdentity(new[]
    //        {
    //        new Claim(ClaimTypes.Name, "mrfibuli"),
    //    }, "Fake authentication type");

    //        var user = new ClaimsPrincipal(identity);

    //        return Task.FromResult(new AuthenticationState(user));
    //    }
    //}





}
