using Microsoft.EntityFrameworkCore;

namespace Caspian.Common.RowNumber
{
    public static class DbFunctionsExtensions
    {
        public static int RowNumber(this DbFunctions _, object param1, params bool[] desc)
        {
            throw new InvalidOperationException("This method is for use with Entity Framework Core only and has no in-memory implementation.");
        }
    }
}
