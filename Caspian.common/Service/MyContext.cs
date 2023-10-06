using Caspian.Common.JsonValue;
using Caspian.Common.RowNumber;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace Caspian.Common
{
    public class MyContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //string projectPath = AppDomain.CurrentDomain.BaseDirectory.Split(new String[] { @"bin\" }, StringSplitOptions.None)[0];
            optionsBuilder.UseSqlServer(CS.Con, t =>
            {
                t.AddRowNumberSupport();

            }).EnableSensitiveDataLogging();
            optionsBuilder.UseLazyLoadingProxies(false);
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDbFunction(typeof(JsonExtensions).GetMethod(nameof(JsonExtensions.JsonValue)))
               .HasTranslation(e => new SqlFunctionExpression("JSON_VALUE", e, true, new[] { true, false }, typeof(String), null));
            base.OnModelCreating(modelBuilder);
        }
    }
}