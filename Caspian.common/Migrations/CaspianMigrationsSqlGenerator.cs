using Microsoft.EntityFrameworkCore.Update;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace Caspian.Common.Migrations
{
    public class CaspianMigrationsSqlGenerator : SqlServerMigrationsSqlGenerator
    {
        TableOperationKind tableOperationKind;
        
        public CaspianMigrationsSqlGenerator(MigrationsSqlGeneratorDependencies dependencies, ICommandBatchPreparer commandBatchPreparer)
            : base(dependencies, commandBatchPreparer)
        {

        }

        protected override void Generate(AlterColumnOperation operation, IModel model, MigrationCommandListBuilder builder)
        {
            if (operation.ComputedColumnSql == null && operation.OldColumn.ComputedColumnSql == null)
            {
                if (operation.ClrType == typeof(string) && operation.OldColumn.ClrType == typeof(string))
                {
                    builder.AppendLine("DECLARE @maxLength int");
                    builder.AppendLine($"set @maxLength = (select max(Len({operation.Name})) from {operation.Schema}.{operation.Table})");
                    builder.AppendLine($"if @maxLength < {operation.MaxLength}");
                    builder.AppendLine($"set @maxLength = {operation.MaxLength}");
                    builder.AppendLine("DECLARE  @sqlCommand varchar(1000)");
                    var str = $"Set @sqlCommand = 'Alter Table {operation.Schema}.{operation.Table} Alter column {operation.Name} nvarchar(' + CONVERT(varchar(4), @maxLength) + ')";
                    if (!operation.IsNullable)
                        str += " NOT NULL";
                    str += "'";
                    builder.AppendLine(str);
                    builder.AppendLine("exec(@sqlCommand)");
                    builder.AppendLine(Dependencies.SqlGenerationHelper.StatementTerminator);
                    EndStatement(builder);
                    return;
                }
            }
            base.Generate(operation, model, builder);
        }

        protected override void Generate(DropColumnOperation operation, IModel model, MigrationCommandListBuilder builder, bool terminate = true)
        {
            DropDefaultConstraint(operation.Schema, operation.Table, operation.Name, builder);
            builder.AppendLine("DECLARE @sqlCommand varchar(1000);");
            builder.AppendLine($"set @sqlCommand = 'Select count(1) from (select distinct {operation.Name} from {operation.Schema}.{operation.Table}) as t'");
            builder.AppendLine("declare @result table ([rowcount] int);");
            builder.AppendLine("insert into @result ([rowcount])");
            builder.AppendLine("EXEC (@sqlCommand)");
            builder.AppendLine("declare @rowcount int = (select top (1) [rowcount] from @result);");
            builder.AppendLine("if @rowcount > 1");
            var errorMessage = $"Column \"{operation.Name}\" in Table \"{operation.Schema}.{operation.Table}\" has more than a values and can not be dropped";
            builder.AppendLine($"throw 51000, '{errorMessage}', 1");
            builder.AppendLine("else");
            builder.AppendLine($"exec ('alter table {operation.Schema}.{operation.Table} drop column {operation.Name}')");
            builder.AppendLine(Dependencies.SqlGenerationHelper.StatementTerminator);
            EndStatement(builder);
        }

        protected override void Generate(DropTableOperation operation, IModel model, MigrationCommandListBuilder builder, bool terminate)
        {
            builder.AppendLine("DECLARE @sqlCommand varchar(1000);");
            builder.AppendLine($"set @sqlCommand = 'Select count(1) from {operation.Schema}.{operation.Name}'");
            builder.AppendLine("declare @result table ([rowcount] int);");
            builder.AppendLine("insert into @result ([rowcount])");
            builder.AppendLine("EXEC (@sqlCommand)");
            builder.AppendLine("declare @rowcount int = (select top (1) [rowcount] from @result);");
            builder.AppendLine("if @rowcount > 0");
            var errorMessage = $"Table \"{operation.Schema}.{operation.Name}\" has records and can not be dropped";
            builder.AppendLine($"throw 51001, '{errorMessage}', 1");
            builder.AppendLine("else");
            builder.AppendLine($"exec ('drop table {operation.Schema}.{operation.Name}')");

            builder.AppendLine(Dependencies.SqlGenerationHelper.StatementTerminator);
            EndStatement(builder);
        }

        protected override void CreateTableColumns(CreateTableOperation operation, IModel model, MigrationCommandListBuilder builder)
        {
            ///Create Column(s) on Create Table
            tableOperationKind = TableOperationKind.Create;
            base.CreateTableColumns(operation, model, builder);
        }

        protected override void Generate(CreateTableOperation operation, IModel model, MigrationCommandListBuilder builder, bool terminate = true)
        {
            if (operation.Name != "__EFMigrationsHistory")
            {

            }
            base.Generate(operation, model, builder, terminate);
        }

        protected override void Generate(AddColumnOperation operation, IModel model, MigrationCommandListBuilder builder, bool terminate)
        {
            //Create Column on Alter Table
            base.Generate(operation, model, builder, terminate);
        }
    }

    enum TableOperationKind
    {
        Create,
        Alter
    }
}
