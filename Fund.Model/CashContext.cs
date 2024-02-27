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
        public DbSet<Treasury> Treasuries { get; set; }
        public DbSet<CashBox> CashBoxes { get; set; }
        public DbSet<Leakage> Leakages { get; set; }
        public DbSet<Over> Overs { get; set; }
        public DbSet<CashBoxType> CashBoxTypes { get; set; }
        public DbSet<Currency> Currencies { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AccountHolder>(b => 
            {
                b.HasKey(t => t.Id);
                b.HasOne<User>(t => t.User).WithOne().IsRequired();
                b.Property(t => t.Code).IsRequired();
                b.Property(t => t.Status).IsRequired();
                b.Property(t => t.Description).HasMaxLength(200).IsRequired(false);

                b.ToTable("AccountHolders", "Cash")
                .HasDiscriminator<AccountHolderTypeEnum>("Type")
                .HasValue<Cashier>(AccountHolderTypeEnum.Cashier)
                .HasValue<Treasurer>(AccountHolderTypeEnum.Treasurer);
            });

            modelBuilder.Entity<Account>(b =>
            {
                b.HasKey(t => t.Id);
                
                b.HasOne<CashBoxType>(t => t.CashBoxType).WithOne().IsRequired();
                b.HasOne<AccountHolder>(t => t.AccountHolder).WithOne().IsRequired();

                b.Property(t => t.Title).HasMaxLength(50).IsRequired();
                b.Property(t => t.FloorLimit).IsRequired();
                b.Property(t => t.IsReferenceCashBox).IsRequired(false);
                b.Property(t => t.Status).IsRequired();
                b.Property(t => t.Description).HasMaxLength(200).IsRequired(false);

                b.ToTable("Accounts", "Cash")
                .HasDiscriminator<AccountTypeEnum>("Type")
                .HasValue<CashBox>(AccountTypeEnum.Cashbox)
                .HasValue<Treasury>(AccountTypeEnum.Treasury)
                .HasValue<Leakage>(AccountTypeEnum.Leakage)
                .HasValue<Over>(AccountTypeEnum.Over);
            });

            modelBuilder.Entity<CashBoxType>(b =>
            {
                b.HasKey(t => t.Id);

                b.HasOne<Currency>(t => t.Currency).WithOne().IsRequired();
                
                b.Property(t => t.Name).HasMaxLength(50).IsRequired();
                b.Property(t => t.InternationalName).HasMaxLength(50).IsRequired(false);
                b.Property(t => t.Description).HasMaxLength(200).IsRequired(false);

                b.ToTable("CashBoxTypes", "Cash");
            });

            modelBuilder.Entity<Currency>(b =>
            {
                b.HasKey(t => t.Id);

                b.Property(t => t.Code).HasMaxLength(50).IsRequired();
                b.Property(t => t.Name).HasMaxLength(50).IsRequired();

                b.ToTable("Currencies", "Cash");
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
