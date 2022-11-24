using Caspian.Common;
using System.Reflection;
using Caspian.Common.Extension;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.Engine
{
    /// <summary>
    /// این کلاس برای فیلدهای دینامیک مورد استفاده قرار می گیرد
    /// </summary>
    public class DynamicFieldEngin
    {
        public string GetDynamicFieldForeignKey(Type mainType, string enTitle)
        {
            if (enTitle.HasValue())
                mainType = mainType.GetMyProperty(enTitle).PropertyType;
            var attr = mainType.GetCustomAttribute<DynamicFieldAttribute>();
            if (attr != null)
            {
                foreach (var info in attr.OtherType.GetProperties())
                {
                    if (info.PropertyType != mainType)
                    {
                        var foreignKey = info.GetCustomAttribute<ForeignKeyAttribute>();
                        if (foreignKey != null)
                            return foreignKey.Name;
                    }
                }
            }
            return null;
        }
    }
}
