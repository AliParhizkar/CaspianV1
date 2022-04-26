using System;
using Microsoft.EntityFrameworkCore;

namespace Caspian.Common.RowNumber
{
    public static class DbFunctionsExtensions
    {
        public static long RowNumber(this DbFunctions _, object param1, params bool[] desc)
        {
            throw new InvalidOperationException("This method is for use with Entity Framework Core only and has no in-memory implementation.");
        }

        public static long RowNumber(this DbFunctions _, object param1, object param2, params bool[] desc)
        {
            throw new InvalidOperationException("This method is for use with Entity Framework Core only and has no in-memory implementation.");
        }

        public static long RowNumber(this DbFunctions _, object param1, object param2, object param3, params bool[] desc)
        {
            throw new InvalidOperationException("This method is for use with Entity Framework Core only and has no in-memory implementation.");
        }

        public static long RowNumber(this DbFunctions _, object param1, object param2, object param3, object param4, params bool[] desc)
        {
            throw new InvalidOperationException("This method is for use with Entity Framework Core only and has no in-memory implementation.");
        }

        public static long RowNumber(this DbFunctions _, object param1, object param2, object param3, object param4, object param5, params bool[] desc)
        {
            throw new InvalidOperationException("This method is for use with Entity Framework Core only and has no in-memory implementation.");
        }
    }
}
