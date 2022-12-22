using Caspian.Common;
using Caspian.Common.Service;
using Microsoft.Extensions.DependencyInjection;

namespace Caspian.Engine.Service
{
    public class DataModelFieldService : SimpleService<DataModelField>
    {
        public DataModelFieldService(IServiceProvider provider)
            :base(provider)
        {
            RuleFor(t => t.EntityFullName).Custom(t => t.EntityFullName.HasValue() && t.FieldType.HasValue, "نوع موجودیت و نوع فیلد فقط یکی می تواند پر باشد")
                .Custom(t => !t.EntityFullName.HasValue() && t.FieldType == null, "نوع موجودیت و نوع فیلد یکی باید پر باشد")
                .UniqAsync("پارامتری از نوع این موجودیت در مدل داده ای وجود دارد");
            
            
            RuleFor(t => t.FieldType).Custom(t => t.EntityFullName.HasValue() && t.FieldType.HasValue, "نوع موجودیت و نوع فیلد فقط یکی می تواند پر باشد")
                .Custom(t => !t.EntityFullName.HasValue() && t.FieldType == null, "نوع موجودیت و نوع فیلد یکی باید پر باشد");
            
            RuleFor(t => t.Title).Required().UniqAsync(t => t.DataModelId, "آبجکتی با این عنوان در فرم ثبت شده است");
            
            RuleFor(t => t.FieldName).Required().UniqAsync(t => t.DataModelId, "آبجکتی با این عنوان در فرم ثبت شده است")
                .CustomValue(t => t.IsValidIdentifire(), "برای تعریف متغیر فقط از کاراکترهای لاتین و یا اعداد استفاده کنید.");
        }

        public static string GetControlTypeName(DataModelFieldType type)
        {
            switch (type)
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
