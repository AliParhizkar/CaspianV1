using Engine.Model;
using Caspian.Common;
using Caspian.Engine.Model;
using Microsoft.EntityFrameworkCore;

namespace Demo.Model
{
    public class Context: CaspianContext
    {
        public DbSet<Menu> Menus { get; set; }

        public DbSet<MenuCategory> MenuCategories { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<ProductCategory> ProductCategories { get; set; }

        public DbSet<Customer> Customers { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<OrderDetail> OrderDetails { get; set; }

        public DbSet<CustomerAddress> CustomerAddresses { get; set; }

        public DbSet<Country> Countries { get; set; }

        public DbSet<City> Cities { get; set; }

        public DbSet<Warehouse> Warehouses { get; set; }

        public DbSet<WarehouseReceipt> Receipts { get; set; }

        public DbSet<ReceiptDetail> ReceiptDetails { get; set; }

        public DbSet<OrganUnit> OrganUnits { get; set; }

        public DbSet<PersianDateConvertor> PersianDatesConvertor { get; set; }

        public DbSet<Meeting> Lectures { get; set; }    

        public DbSet<Test> Tests { get; set; }

        public DbSet<Scope> Scopes { get; set; } 

        public DbSet<Evaluation> Evaluations { get; set; }

        public DbSet<CourseStudy> CourseStudies { get; set; }

        public DbSet<SimpleData> SimpleDatas { get; set; }

        public DbSet<Employee> Employees { get; set; }

        public DbSet<ProductDescription> ProductDescriptions { get; set; }
    }
}
