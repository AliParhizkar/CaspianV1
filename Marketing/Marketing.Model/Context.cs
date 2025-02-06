using Caspian.Common;
using Microsoft.EntityFrameworkCore;

namespace Marketing.Model
{
    public class Context : CaspianContext
    {
        public DbSet<Secretariat> Secretariats { get; set; }

        public DbSet<ProductCategory> ProductCategories { get; set; }
    }
}
