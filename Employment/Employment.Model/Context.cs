using Caspian.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Employment.Model
{
    public class Context: MyContext
    {
        public DbSet<ChildrenProperties> ChildrenProperties { get; set; } 

        public DbSet<City> Cities { get; set; }

        public DbSet<EducationRecords> Educations { get; set; }

        public DbSet<ParametricEmploymentOrder> EmploymentOrders { get; set; }

        public DbSet<MarriageProperties> MarriageProperties { get; set; }

        public DbSet<Province> Provinces { get; set; }

        public DbSet<WorkHistory> WorkHistory { get; set; }

        public DbSet<OrganUnit> OrganUnits { get; set; }

        public DbSet<Occupation> Occupations { get; set; }

        public DbSet<CareerField> CareerFields { get; set; }

        public DbSet<OrganPost> OrganPosts { get; set; }

        public DbSet<BaseNumber> BaseNumbers { get; set; }

        public DbSet<BaseStudyFactor> BaseStudyFactors { get; set; }

        public DbSet<ExtraFactor> ExtraFactors { get; set; }

        public DbSet<EmploymentOrderDynamicParameterValue> EmploymentOrderDynamicParameterValues { get; set; }
    }
}
