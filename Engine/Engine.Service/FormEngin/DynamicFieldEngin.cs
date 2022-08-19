using Caspian.Common;
using System.Reflection;
using Caspian.Common.Extension;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.Engine
{
    /// <summary>
    /// این کلاس برای فیلدهای دینامیک مورد استفاده قرار می گیرد
    /// </summary>
    public class DynamicFieldEngin
    {
        public string? GetDynamicFieldForeignKey(Type mainType, string enTitle)
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

        public async Task<IList<DynamicParameterType>> GetDynamicItems(Type mainType, int? formId)
        {
            throw new NotImplementedException("خطای عدم پیاده سازی");
            //IList<DynamicParameterType> tempList = null;
            //var dynamicFieldAttr = mainType.GetCustomAttribute<DynamicFieldAttribute>();
            //if (dynamicFieldAttr != null)
            //{
            //    using (var context = new Context())
            //    {
            //        var query = context.DynamicParameters.AsQueryable();
            //        if (formId.HasValue)
            //            query = query.Where(t => t.FormId == formId);
            //        else
            //            query = query.Where(t => t.Form.FormGroup.Namespace == mainType.Namespace && t.Form.FormGroup.ClassName == mainType.Name);
            //        if (formId.HasValue)
            //        {
            //            tempList = await query.Select(t => new DynamicParameterType()
            //            {
            //                Id = t.Id,
            //                Title = t.Title
            //            }).ToListAsync();
            //        }
            //        else
            //        {
            //            tempList = await query.Select(t => new DynamicParameterType
            //            {
            //                FormId = t.FormId,
            //                Title = t.Form.Title,
            //            }).Distinct().ToListAsync();
            //        }
            //    }
            //}
            //return tempList;
        }

        //public Type DynamicType(IList<ReportParam> dynamicFields, Type type = null)
        //{
        //    if (!dynamicFields.Any())
        //        return type;
        //    var list = new List<DynamicProperty>();
        //    if (type != null)
        //    {
        //        //foreach(var info in type.GetProperties())
        //    }
        //    return null;
        //}
    }

    public class DynamicParameterType
    {
        public int Id { get; set; }

        public int ParameterId { get; set; }

        public string Title { get; set; }

        public string Value { get; set; }

        public string Display { get; set; }

        public  ControlType ControlType { get; set; }
    }
}
