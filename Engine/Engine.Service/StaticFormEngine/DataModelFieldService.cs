using Caspian.Common;
using Caspian.Common.Service;
using Microsoft.EntityFrameworkCore;

namespace Caspian.Engine.Service
{
    public class DataModelFieldService : BaseService<DataModelField>
    {
        public DataModelFieldService(IServiceProvider provider)
            :base(provider)
        {
            RuleFor(t => t.EntityFullName).Custom(t => t.EntityFullName.HasValue() && t.FieldType.HasValue, "نوع موجودیت و نوع فیلد فقط یکی می تواند پر باشد")
                .Custom(t => !t.EntityFullName.HasValue() && t.FieldType == null, "نوع موجودیت و نوع فیلد یکی باید پر باشد")
                .UniqAsync("پارامتری از نوع این موجودیت در مدل داده ای وجود دارد");
            
            
            RuleFor(t => t.FieldType).Custom(t => t.EntityFullName.HasValue() && t.FieldType.HasValue, "نوع موجودیت و نوع فیلد فقط یکی می تواند پر باشد")
                .Custom(t => !t.EntityFullName.HasValue() && t.FieldType == null, "نوع موجودیت و نوع فیلد یکی باید پر باشد")
                .CustomAsync(async t => 
                {
                    if (t.FieldType != DataModelFieldType.MultiSelect && t.Id > 0)
                        return await new DataModelOptionService(provider).GetAll().AnyAsync(u => u.FieldId == t.Id);
                    return false;
                }, "فیلد دارای گزینه می باشد و امکان تغییر نوع آن وجود ندارد");
            
            RuleFor(t => t.Title).Required().UniqAsync(t => t.DataModelId, "آبجکتی با این عنوان در فرم ثبت شده است");
            RuleFor(t => t.EntityTypeId).Required(t => t.FieldType == DataModelFieldType.Relational)
                .Custom(t => t.EntityTypeId.HasValue && t.FieldType != DataModelFieldType.Relational, "نوع موجودیت باید خالی باشد.");
            RuleFor(t => t.FieldName).Required().UniqAsync(t => t.DataModelId, "آبجکتی با این عنوان در فرم ثبت شده است")
                .CustomValue(t => t.IsValidIdentifire(), "برای تعریف متغیر فقط از کاراکترهای لاتین و یا اعداد استفاده کنید.");
        }

        public static string GetFieldTypeName(DataModelField field)
        {
            if (field.EntityFullName.HasValue())
                return field.EntityFullName;
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
                case DataModelFieldType.Relational:
                    return "int?";
                default: throw new NotImplementedException("خطای عدم پیاده سازی");
            }
        }

        public static string GetControlTypeName(DataModelField field)
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
                default: throw new NotImplementedException("خطای عدم پیاده سازی");
            }
        }
    }

    internal static class StringExtenssion
    {
        public static bool IsValidIdentifire(this string str )
        {
            if (!str.HasValue())
                return false;
            var upper = str.ToUpper();
            if (upper[0] < 'A' || upper[0] > 'Z')
                return true;
            var isInvalid = false;
            foreach(var chr in upper)
            {
                if(chr < '0' || chr > '9' && chr < 'A' || chr > 'Z')
                    isInvalid = true;
            }
            return isInvalid;
        }
    }
}
