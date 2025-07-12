using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.Engine.Model
{
    //[Table("UserClaims", )]
    public class UserClaim : IdentityUserClaim<int> 
    {
        
    }
}
