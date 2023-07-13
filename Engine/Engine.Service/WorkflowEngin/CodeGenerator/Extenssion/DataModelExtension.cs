using System.Text;
using Caspian.Common;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Caspian.Common.Extension;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.Engine.Service
{
    public static class DataModelExtension
    {
        public static string GetCode(this DataModelOption option, int index)
        {
            StringBuilder str = new StringBuilder();
            str.Append($"\t\t[Display(Name = \"{option.Title}\")]\n");
            str.Append($"\t\t{option.Name} = {index}");
            return str.ToString();
        }

        public static string GetEnumCode(this DataModelField field)
        {
            StringBuilder str = new StringBuilder();
            var options = field.DataModelOptions;
            if (options.Count > 0)
            {
                var index = 1;
                str.Append("\tpublic enum " + field.FieldName + "\n\t{\n");
                foreach (var option in options)
                {
                    str.Append(option.GetCode(index));
                    if (index != options.Count)
                        str.Append(",\n");
                    str.Append("\n");
                    index++;
                }
                str.Append("\t}");
            }
            return str.ToString();
        }

        public static string GetFieldInitializeCode(this DataModel model)
        {
            StringBuilder str = new StringBuilder();
            foreach (var field in model.Fields)
            {
                if (field.EntityFullName.HasValue())
                    str.Append($"\t\t\t{field.FieldName} = new {field.EntityFullName}();\n");
            }
            return str.ToString();
        }

        public static string GetFieldType(this DataModelField field)
        {
            if (field.EntityFullName.HasValue())
                return field.EntityFullName;
            if (field.FieldType == DataModelFieldType.Relational)
                return "int?";
            switch (field.FieldType.Value)
            {
                case DataModelFieldType.String:
                    return "string";
                case DataModelFieldType.Integer:
                    return "int?";
                case DataModelFieldType.Decimal:
                    return "decimal?";
                case DataModelFieldType.Date:
                    return "DateTime?";
                case DataModelFieldType.MultiOptions:
                    return field.FieldName;
                default: throw new NotImplementedException("خطای عدم پیاده سازی");
            }
        }

        public static string GetFieldsCode(this DataModel model)
        {
            var str = new StringBuilder();
            str.Append("\n\t\t//Fields\n");
            foreach (var field in model.Fields)
            {
                var typeName = field.GetFieldType();
                str.Append($"\t\t{typeName} {field.FieldName};\n");
            }
            return str.ToString();
        }
    }
}
