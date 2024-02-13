using Caspian.Engine.Model;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Model
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
         : IdentityDbContext<User, Role, int, UserClaim, UserMembership, UserLogin, RoleClaim, UserToken>(options)
    {
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>().ToTable("Users", "cmn");
            builder.Entity<Role>().ToTable("Roles", "cmn");
            builder.Entity<UserMembership>().ToTable("UsersMembership", "cmn");
            builder.Entity<UserClaim>().ToTable("UserClaims", "cmn");
            builder.Entity<RoleClaim>().ToTable("RoleClaims", "cmn");
            builder.Entity<UserLogin>().ToTable("UserLogins", "cmn");
            builder.Entity<UserToken>().ToTable("UserTokens", "cmn");

            builder.Entity<UserMembership>().HasKey(t => t.Id);
        }
    }
}
