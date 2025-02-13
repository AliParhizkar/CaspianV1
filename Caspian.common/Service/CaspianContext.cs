using System.Reflection;
using Caspian.Common.Extension;
using Caspian.Common.JsonValue;
using Caspian.Common.RowNumber;
using Caspian.Common.Migrations;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Migrations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

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
            }).ReplaceService<IMigrationsSqlGenerator, CaspianMigrationsSqlGenerator>()
                .EnableSensitiveDataLogging();
            optionsBuilder.UseLazyLoadingProxies(false);
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var types = modelBuilder.Model.GetEntityTypes().Select(t => t.ClrType);
            var assemblyName = this.GetType().Assembly.GetName().Name;
            foreach (var type in types)
            {
                var pKeyName = type.GetPrimaryKey(true)?.Name;
                foreach (var property in type.GetProperties())
                {
                    if (property.PropertyType.FullName == "Caspian.Engine.Model.User")
                    {
                        continue;
                    }
                    var foreignKey = property.GetCustomAttribute<ForeignKeyAttribute>();
                    if (foreignKey != null)
                    {
                        if (foreignKey.Name == pKeyName)
                        {
                            var info = property.PropertyType.GetProperties().Single(t => t.PropertyType == type);
                            modelBuilder.Entity(property.PropertyType)
                                .HasOne(info.Name)
                                .WithOne(property.Name)
                                .IsRequired(false)
                                .OnDelete(DeleteBehavior.Cascade);
                        }
                        else
                        {
                            var mainyproperties = property.PropertyType.GetProperties().Where(t => t.PropertyType.IsCollectionType(type));
                            var count = mainyproperties.Count();
                            var relationsCount = count;
                            if (count > 1)
                            {
                                mainyproperties = mainyproperties.Where(t => t.GetCustomAttribute<InversePropertyAttribute>()?.Property == property.Name);
                                count = mainyproperties.Count();
                            }

                            if (count != 1)
                            {
                                if (relationsCount == 0)
                                    throw new CaspianException($"On Type {property.PropertyType} we shoul have a property of type ICollection<{type.Name}>");
                                else
                                    throw new CaspianException($"On Type {property.PropertyType} we have many properties of type ICollection<{type.Name}> and we should use InverseProperty({property.Name}) Relation Coun:{relationsCount}");
                            }

                            modelBuilder.Entity(type)
                                .HasOne(property.Name)
                                .WithMany(mainyproperties.Single().Name)
                                .OnDelete(DeleteBehavior.NoAction);

                        }
                    }
                }

                
                if (!assemblyName.Equals("Engine.Model", StringComparison.OrdinalIgnoreCase))
                {
                    TableAttribute tableAttribute = type.GetCustomAttribute<TableAttribute>();
                    if (tableAttribute?.Schema == "cmn")
                    {
                        modelBuilder.Entity(type).ToTable(tableAttribute.Name, tableAttribute.Schema, t =>
                        {
                            t.ExcludeFromMigrations();
                            
                        });
                    }
                }

                foreach (var property in type.GetProperties())
                {
                    if (property.PropertyType == typeof(string))
                    {
                        var maxLength = property.GetCustomAttribute<MaxLengthAttribute>();
                        if (maxLength == null)
                        {
                            var columnAttribute = type.GetCustomAttribute<ColumnAttribute>();
                            if (columnAttribute?.TypeName == null)
                                modelBuilder.Entity(type).Property(property.Name).HasMaxLength(50);
                        }
                        var requiredAttribute = property.GetCustomAttribute<RequiredAttribute>();
                        modelBuilder.Entity(type).Property(property.Name).IsRequired(requiredAttribute != null);
                    }
                    else
                    {
                        var propertyType = property.PropertyType.GetUnderlyingType();
                        var floatTypes = new Type[] { typeof(float), typeof(double), typeof(decimal) };
                        if (floatTypes.Contains(propertyType))
                        {
                            var precisionAttribute = property.GetCustomAttribute<PrecisionAttribute>();
                            if (precisionAttribute == null)
                            {
                                var columnAttribute = type.GetCustomAttribute<ColumnAttribute>();
                                if (columnAttribute?.TypeName == null)
                                    modelBuilder.Entity(type).Property(property.Name).HasPrecision(10, 2);
                            }
                        }
                        else if (propertyType == typeof(DateTime))
                        {
                            var columnAttribute = type.GetCustomAttribute<ColumnAttribute>();
                            if (columnAttribute?.TypeName == null)
                                modelBuilder.Entity(type).Property(property.Name).HasColumnType("datetime(2)");
                        }
                    }
                    
                    var attr = property.GetCustomAttribute<ComputedColumnSqlAttribute>();
                    if (attr != null)
                    {
                        var sql = attr.GetSql();
                        modelBuilder.Entity(type).Property(property.Name).HasComputedColumnSql(sql);
                    }
                }
            }

            modelBuilder.HasDbFunction(typeof(JsonExtensions).GetMethod(nameof(JsonExtensions.JsonValue)))
               .HasTranslation(e => new SqlFunctionExpression("JSON_VALUE", e, true, new[] { true, false }, typeof(String), null));
            base.OnModelCreating(modelBuilder);
        }
    }
}