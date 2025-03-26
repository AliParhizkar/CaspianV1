using Caspian.Common;
using Microsoft.EntityFrameworkCore;

namespace Marketing.Model
{
    public class Context : CaspianContext
    {
        public DbSet<Customer> Customers { get; set; }
        
        public DbSet<CustomerAddress> CustomerAddresses { get; set; }

        public DbSet<CustomerGroup> CustomerGroups { get; set; }
        
        public DbSet<CustomerGroupMemberShip> CustomerGroupMemberShips { get; set; }
        
        public DbSet<Config> MerchantConfigs { get; set; }

        public DbSet<Order> Orders { get; set; }
        
        public DbSet<OrderDetail> OrderDetails { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<ProductCategory> ProductCategories { get; set; }

        public DbSet<ProductDescription> ProductDescriptions { get; set; }  
    }
}
