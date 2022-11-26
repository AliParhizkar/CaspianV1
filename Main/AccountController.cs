using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Caspian.Engine.Model;
using Stimulsoft.System.Windows.Forms;

namespace Main
{
    [ApiController]
    [Route("/[controller]")]
    public class AccountController : Controller
    {
        [HttpPost]
        public async Task<ActionResult> Login([FromForm] string userName, [FromForm] string password)
        {
            using var context = new Context();
            var user = context.Users.SingleOrDefault(t => t.UserName == userName && t.Password == password);
            if (user != null)
            {
                ClaimsIdentity claimsIdentity = new ClaimsIdentity(new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
                }, "Identity.Application12345");
                ClaimsPrincipal claims = new ClaimsPrincipal(claimsIdentity);
                await HttpContext.SignInAsync(claims, new AuthenticationProperties()
                {
                    IsPersistent = false
                });
                return Redirect("/");
            }
            ViewBag.Message = "نام کاربری یا کلمه ی عبور نامعتبر است";
            return View();
        }
    }
}
