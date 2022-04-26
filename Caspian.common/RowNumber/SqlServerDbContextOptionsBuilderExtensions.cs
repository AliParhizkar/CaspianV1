using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Caspian.Common.RowNumber
{
    public static class SqlServerDbContextOptionsBuilderExtensions
    {
        public static SqlServerDbContextOptionsBuilder AddRowNumberSupport(
                   this SqlServerDbContextOptionsBuilder sqlServerOptionsBuilder)
        {
            var infrastructure = (IRelationalDbContextOptionsBuilderInfrastructure)
                                 sqlServerOptionsBuilder;
            var builder = (IDbContextOptionsBuilderInfrastructure)
                              infrastructure.OptionsBuilder;


            var extension = infrastructure.OptionsBuilder.Options
                                          .FindExtension<SqlServerDbContextOptionsExtension>()
                            ?? new SqlServerDbContextOptionsExtension();
            builder.AddOrUpdateExtension(extension);

            return sqlServerOptionsBuilder;
        }
    }
}
