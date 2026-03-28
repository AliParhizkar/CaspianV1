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
            var subsystemData = GetType().Assembly.GetSubsystemMetaData();
            var schema = subsystemData.Schema;
            optionsBuilder.UseSqlServer(CS.Con, t =>
            {
                t.AddRowNumberSupport();
                t.MigrationsHistoryTable("__EFMigrationsHistory", schema);
            }).ReplaceService<IMigrationsSqlGenerator, CaspianMigrationsSqlGenerator>()
                .EnableSensitiveDataLogging();
            optionsBuilder.UseLazyLoadingProxies(false);

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var assembly = this.GetType().Assembly;
            var schema = assembly.GetSubsystemMetaData().Schema;
            var allTypes = assembly.GetTypes().Where(t => t.GetCustomAttribute<TableAttribute>() != null);
            foreach (var type in allTypes)
                modelBuilder.Entity(type);
            var types = modelBuilder.Model.GetEntityTypes().Select(t => t.ClrType);
            foreach (var type in types)
            {
                TableAttribute tableAttribute = type.GetCustomAttribute<TableAttribute>();
                if (tableAttribute == null)
                    throw new CaspianException($"Type {type.FullName} is a entity and haven't any TableAttribute");
                if (!tableAttribute.Schema.HasValue())
                    throw new CaspianException($"in Type {type.FullName} schema is null or empty");
                if (tableAttribute.Schema != schema)
                {
                    modelBuilder.Entity(type).ToTable(tableAttribute.Name, tableAttribute.Schema, t =>
                    {
                        t.ExcludeFromMigrations();
                    });
                    continue;
                }
                var pKeyName = type.GetPrimaryKey(true)?.Name;
                foreach (var property in type.GetProperties())
                {
                    var names = new string[] { "Caspian.Engine.Model.User", "Caspian.Engine.Model.PersianDateTable"};
                    if (names.Contains(property.PropertyType.FullName))
                        continue;
                    var type1 = property.PropertyType;
                    var foreignKey = property.GetCustomAttribute<ForeignKeyAttribute>();
                    if (type1.IsValueType || type1 == typeof(string) || type1 == typeof(byte[]))
                    {
                        if (foreignKey != null)
                            throw new CaspianException($"Only properties of type entity can have ForeignKeyAttribute: In type {type.Name} property {property.Name} have ForeignKeyAttribute");
                    }
                    else if (!type1.IsCollectionType())
                    {
                        if (schema == type1.GetCustomAttribute<TableAttribute>().Schema)
                        {
                            ///Check relations between this entity and others entities 
                            if (type1.IsEnumerableType())
                            {
                                ///1-n relationship. check "Details" property is exist
                                var detailsType = type1.GenericTypeArguments[0];
                                detailsType.GetForeignKey(type, property.GetCustomAttribute<InversePropertyAttribute>()?.Property);
                            }
                            else if (foreignKey == null)
                            {
                                /// 1-1 relation check "Master" property is exist
                                var relationProperty = type1.GetProperties().SingleOrDefault(t => t.PropertyType == type);
                                if (relationProperty == null)
                                    throw new CaspianException($"For a 1-1 relation, In type {type1.Name} we should have a property of type {type.Name}");
                                var relationAttr = relationProperty.GetCustomAttribute<ForeignKeyAttribute>();
                                if (relationAttr == null || relationAttr.Name != type1.GetPrimaryKey(true).Name)
                                    throw new CaspianException($"For a 1-1 relation, Property {relationProperty.Name} in type {type1.Name} should has a ForeignKeyAttribute: ForeignKey(nameof({type1.GetPrimaryKey(true).Name}))");
                            }
                            else
                            {
                                if (foreignKey.Name == pKeyName)
                                {
                                    ///1-1 relation check "Detail" property is exist
                                    if (property.PropertyType.GetProperties().SingleOrDefault(t => t.PropertyType == type) == null)
                                        throw new CaspianException($"For a 1-n relation, In type {property.PropertyType.Name} we should have a property of type {type.Name} please add it");
                                }
                                if (foreignKey.Name == pKeyName)
                                {
                                    /// Delete all relation on 1 to 1 relationship
                                    var info = property.PropertyType.GetProperties().Single(t => t.PropertyType == type);
                                    modelBuilder.Entity(property.PropertyType)
                                        .HasOne(info.Name)
                                        .WithOne(property.Name)
                                        .IsRequired(false)
                                        .OnDelete(DeleteBehavior.Cascade);
                                }
                                else
                                {
                                    var mainProperties = property.PropertyType.GetProperties().Where(t => t.PropertyType.IsCollectionType(type));
                                    var count = mainProperties.Count();
                                    var relationsCount = count;
                                    if (count > 1)
                                    {
                                        mainProperties = mainProperties.Where(t => t.GetCustomAttribute<InversePropertyAttribute>()?.Property == property.Name);
                                        count = mainProperties.Count();
                                    }
                                    if (count != 1)
                                    {
                                        if (relationsCount == 0)
                                            throw new CaspianException($"On Type {property.PropertyType} we should have a property of type ICollection<{type.Name}>");
                                        else
                                            throw new CaspianException($"On Type {property.PropertyType} we have many properties of type ICollection<{type.Name}> and we should use InverseProperty({property.Name}) Relation Coun:{relationsCount}");
                                    }
                                    modelBuilder.Entity(type)
                                        .HasOne(property.Name)
                                        .WithMany(mainProperties.Single().Name)
                                        .OnDelete(DeleteBehavior.NoAction);
                                }
                            }
                        }
                        else
                        {
                            var otherType = typeof(ICollection<>).MakeGenericType(type);
                            modelBuilder.Entity(type)
                                .HasOne(property.Name)
                                .WithMany()
                                .OnDelete(DeleteBehavior.NoAction);
                        }
                    }
                }
                var baseType = type;
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
                                modelBuilder.Entity(type).Property(property.Name).HasColumnType("datetime2").HasPrecision(2);
                        }
                    }
                    
                    var attr = property.GetCustomAttribute<ComputedSqlColumnAttribute>();
                    if (attr != null)
                    {
                        var sql = attr.GetSql();
                        var entityProperty = modelBuilder.Entity(type).Property(property.Name).HasComputedColumnSql(sql);
                    }
                }
            }

            modelBuilder.HasDbFunction(typeof(JsonExtensions).GetMethod(nameof(JsonExtensions.JsonValue)))
               .HasTranslation(e => new SqlFunctionExpression("JSON_VALUE", e, true, new[] { true, false }, typeof(String), null));
            
            base.OnModelCreating(modelBuilder);
        }
    }
}