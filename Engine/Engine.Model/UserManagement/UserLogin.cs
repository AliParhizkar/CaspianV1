using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.Engine.Model
{
    [Table("UsersLogins", Schema = "cmn")]
    public class UserLogin : IdentityUserLogin<int> 
    {
        [Key]
        public int Id { get; set; }

        public string IPAddress { get; set; }

        public DateTime LoginDate { get; set; }

        public string PageUrl { get; set; }
    }
}
