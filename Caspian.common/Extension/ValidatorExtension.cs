using Caspian.Common.Extension;
using FluentValidation;
using FluentValidation.Results;

namespace Caspian.Common.Extension
{
    public static class ValidatorExtension
    {
        public static Task<ValidationResult> ValidateAsync<TModel>(this IValidator<TModel> validator, TModel instance, Type detailType)
            where TModel : class
        {
            return validator.ValidateAsync(instance, t =>
            {
                t.IncludeRuleSets("");
                var list = new List<string>();
                AddPropetiesToList(typeof(TModel), null, list);
                if (detailType != typeof(TModel))
                {
                    var propertyName = typeof(TModel).GetProperties().Single(t => t.PropertyType == detailType).Name;
                    AddPropetiesToList(detailType, propertyName, list);
                }
                t.IncludeProperties(list.ToArray());
            });
        }

        static void AddPropetiesToList(Type type, string propertyName, IList<string> list)
        {
            var pKey = type.GetPrimaryKey();
            foreach (var info in (type as Type).GetProperties().Where(t => t != pKey))
            {
                if (info.PropertyType.GetUnderlyingType().IsValueType || info.PropertyType == typeof(string) || info.PropertyType == typeof(byte[]))
                {
                    if (propertyName == null)
                        list.Add(info.Name);
                    else
                        list.Add($"{propertyName}.{info.Name}");
                }
            }
        }
    }
}
