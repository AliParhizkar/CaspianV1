using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using System.Collections.Generic;

namespace Caspian.Common.RowNumber
{
    public class SqlServerDbContextOptionsExtension : IDbContextOptionsExtension
    {
        public SqlServerDbContextOptionsExtension()
        {

        }

        public string LogFragment => "'RowNumberSupport'=true";

        public DbContextOptionsExtensionInfo Info { get { return new RowNumberOptionsExtensionInfo(this); } }

        public void ApplyServices(IServiceCollection services)
        {
            services.AddSingleton<IMethodCallTranslatorPlugin, SqlServerMethodCallTranslatorPlugin>();
        }

        public void Validate(IDbContextOptions options)
        {

        }
    }
}
