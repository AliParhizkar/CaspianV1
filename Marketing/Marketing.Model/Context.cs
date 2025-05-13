using Caspian.Common;
using Microsoft.EntityFrameworkCore;

namespace Marketing.Model
{
    public class MarketingContext : CaspianContext
    {
        public DbSet<Customer> Customers { get; set; }
        
        public DbSet<CustomerAddress> CustomerAddresses { get; set; }

        public DbSet<CustomerGroup> CustomerGroups { get; set; }
        
        public DbSet<CustomerGroupMembership> CustomerGroupMemberships { get; set; }
        
        public DbSet<Config> MerchantConfigs { get; set; }

        public DbSet<Order> Orders { get; set; }
        
        public DbSet<OrderDetail> OrderDetails { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<ProductCategory> ProductCategories { get; set; }

        public DbSet<ProductDescription> ProductDescriptions { get; set; } 
        
        public DbSet<Topping> Toppings { get; set; }

        public DbSet<ProductTopping> ProductToppings { get; set; }

        public DbSet<OrderDetailTopping> GetOrderDetailToppings {  get; set; }

        public DbSet<PrinterLocation> PrinterLocations { get; set; }

        public DbSet<PrinterProduct> PrinterProducts { get; set; }  

        public DbSet<CustomerAccounting> CustomerAccountings { get; set; }
    }
}
