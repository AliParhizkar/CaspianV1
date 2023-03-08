﻿using Caspian.Common;
using Caspian.Engine.Model;
using Microsoft.EntityFrameworkCore;

namespace Demo.Model
{
    public class Context: MyContext
    {
        public DbSet<Menu> Menus { get; set; }

        public DbSet<MenuCategory> MenuCategories { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<ProductCategory> ProductCategories { get; set; }

        public DbSet<Customer> Customers { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<OrderDeatil> OrderDeatils { get; set; }

        public DbSet<Country> Countries { get; set; }

        public DbSet<City> Cities { get; set; }

        public DbSet<Area> Areas { get; set; }

        public DbSet<Warehouse> Warehouses { get; set; }

        public DbSet<WarehouseReceipt> Receipts { get; set; }

        public DbSet<MaterialReceipt> ProductReceipts { get; set; }

        public DbSet<OrganUnit> OrganUnits { get; set; }
    }
}
