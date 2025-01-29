using Caspian.Common;
using Microsoft.EntityFrameworkCore;

namespace Marketing.Model
{
    public class Context : CaspianContext
    {
        public DbSet<OrganUnit> OrganUnits { get; set; }
    }
}
