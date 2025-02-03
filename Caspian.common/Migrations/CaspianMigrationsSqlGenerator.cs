using Microsoft.EntityFrameworkCore.Update;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace Caspian.Common.Migrations
{
    public class CaspianMigrationsSqlGenerator : SqlServerMigrationsSqlGenerator
    {
        public CaspianMigrationsSqlGenerator(MigrationsSqlGeneratorDependencies dependencies, ICommandBatchPreparer commandBatchPreparer)
            : base(dependencies, commandBatchPreparer)
        {

        }
        TableOperationKind tableOperationKind;


        protected override void ColumnDefinition(AddColumnOperation operation, IModel model, MigrationCommandListBuilder builder)
        {

            if (operation.Table != "__EFMigrationsHistory")
            {
                if (operation.ClrType == typeof(string) && operation.MaxLength == null && operation.ComputedColumnSql == null &&
                    operation.ColumnType.Equals("nvarchar(max)", StringComparison.OrdinalIgnoreCase))
                {
                    if (tableOperationKind == TableOperationKind.Create)
                    {
                        builder.Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Name))
                            .Append(" ")
                            .Append("nvarchar(50)");
                        builder.Append(operation.IsNullable ? " NULL" : " NOT NULL");
                    }

                }
                else
                    base.ColumnDefinition(operation, model, builder);

            }
            else
                base.ColumnDefinition(operation, model, builder);
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

        protected override void Generate(AlterColumnOperation operation, IModel model, MigrationCommandListBuilder builder)
        {
            if (operation.ClrType == typeof(string) && operation.MaxLength == null && operation.ComputedColumnSql == null &&
                    operation.ColumnType.Equals("nvarchar(max)", StringComparison.OrdinalIgnoreCase))
            {
                if (tableOperationKind == TableOperationKind.Create)
                {
                    builder.Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Name))
                        .Append(" ")
                        .Append("nvarchar(50)");
                    builder.Append(operation.IsNullable ? " NULL" : " NOT NULL");
                }
            }
            else
                base.Generate(operation, model, builder);
        }
    }

    enum TableOperationKind
    {
        Create,
        Alter
    }
}
