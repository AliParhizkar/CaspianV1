using System.Reflection;
using Caspian.Common.Extension;
using Caspian.Common.JsonValue;
using Caspian.Common.RowNumber;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.Common
{
    public class CaspianContext : DbContext
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
            var types = this.GetType().Assembly.GetTypes();
            foreach (var type in types)
            {
                var pKeyName = type.GetPrimaryKey(true)?.Name;
                if (pKeyName != null)
                {
                    var fKeyInfo = type.GetProperties().SingleOrDefault(t => t.GetCustomAttribute<ForeignKeyAttribute>()?.Name == pKeyName);
                    if (fKeyInfo != null)
                    {
                        var info = fKeyInfo.PropertyType.GetProperties().Single(t => t.PropertyType == type);
                        modelBuilder.Entity(fKeyInfo.PropertyType)
                            .HasOne(info.Name)
                            .WithOne(fKeyInfo.Name)
                            .IsRequired(false)
                            .OnDelete(DeleteBehavior.Cascade);
                    }
                }
            }
            modelBuilder.HasDbFunction(typeof(JsonExtensions).GetMethod(nameof(JsonExtensions.JsonValue)))
               .HasTranslation(e => new SqlFunctionExpression("JSON_VALUE", e, true, new[] { true, false }, typeof(String), null));
            base.OnModelCreating(modelBuilder);
        }
    }
}