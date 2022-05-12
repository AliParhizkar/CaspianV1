using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Caspian.Common.RowNumber
{
    public class RowNumberOptionsExtensionInfo : DbContextOptionsExtensionInfo
    {
        public RowNumberOptionsExtensionInfo(IDbContextOptionsExtension extension) :
            base(extension)
        {

        }

        public override bool ShouldUseSameServiceProvider(DbContextOptionsExtensionInfo other)
        {
            return true;
        }

        public override bool IsDatabaseProvider { get { return true; } }

        public override string LogFragment { get { return ""; } }


        public override int GetServiceProviderHashCode()
        {
            return 0;
        }

        public override void PopulateDebugInfo(IDictionary<string, string> debugInfo)
        {

        }


    }

}