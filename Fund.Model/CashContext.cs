using Caspian.Common;
using Caspian.Engine.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fund.Model
{
    public class CashContext : MyContext
    {
        public DbSet<Treasurer> Treasurers { get; set; }

        public DbSet<Cashier> Cashiers { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AccountHolder>(b => 
            {
                b.HasKey(t => t.Id);
                b.HasOne<User>(t => t.User).WithOne().IsRequired();
                b.Property(t => t.Description).HasMaxLength(500).IsRequired(false);
                b.Property(t => t.Code).IsRequired();
                b.Property(t => t.Status).IsRequired();

                b.ToTable("AccountHolders", "Cash")
                .HasDiscriminator<AccountHolderTypeEnum>("Type")
                .HasValue<Cashier>(AccountHolderTypeEnum.Cashier)
                .HasValue<Treasurer>(AccountHolderTypeEnum.Treasurer);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
