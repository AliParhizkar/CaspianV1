using Caspian.Common;
using Microsoft.EntityFrameworkCore;

namespace Employment.Model
{
    public class Context: MyContext
    {
        public DbSet<ChildrenProperties> ChildrenProperties { get; set; } 

        public DbSet<City> Cities { get; set; }

        public DbSet<EducationRecords> Educations { get; set; }

        public DbSet<EmploymentOrder> EmploymentOrders { get; set; }

        public DbSet<MarriageProperties> MarriageProperties { get; set; }

        public DbSet<Province> Provinces { get; set; }

        public DbSet<WorkHistory> WorkHistory { get; set; }
    }
}
