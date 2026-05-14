using Caspian.Common;
using Microsoft.EntityFrameworkCore;

namespace Caspian.Engine.Model
{
    public class Context: CaspianContext
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>().Property(t => t.PasswordHash).HasMaxLength(200);
            modelBuilder.Entity<UserClaim>();
        }
    }
}
