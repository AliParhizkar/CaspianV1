using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("UIComponent")]
namespace Caspian.Common.Extension
{
    public static class OtherExtension
    {

        public static TEntity CreateNewEntity<TEntity>(this TEntity entity)
            where TEntity : class
        {
            return CreateNewObjectAndCopy(typeof(TEntity), entity) as TEntity;
        }

        public static object CreateNewObjectAndCopy(Type type, object value)
        {
            if (value == null)
                return null;
            var newObj = Activator.CreateInstance(type);
            foreach (var info in type.GetProperties())
            {
                var type1 = info.PropertyType;
                if (type1.IsValueType || type1.IsNullableType() || type1 == typeof(string) || type1 == typeof(byte[]))
                    info.SetValue(newObj, info.GetValue(value));
                else
                {
                    var tempValue = CreateNewObjectAndCopy(type1, info.GetValue(value));
                    info.SetValue(newObj, tempValue);
                }
            }
            return newObj;
        }

        public static TModel CreateNewSimpleEntity<TModel>(this TModel model)
            where TModel : class
        {
            var newEntity = Activator.CreateInstance<TModel>();
            foreach (var info in typeof(TModel).GetProperties())
            {
                var type = info.PropertyType;
                if (type.IsValueType || type == typeof(string) || type == typeof(byte[]))
                    info.SetValue(newEntity, info.GetValue(model));
            }
            return newEntity;
        }

        public static void CopySimpleProperty<TModel>(this TModel model, TModel newModel)
        {
            foreach (var info in typeof(TModel).GetProperties())
            {
                var type = info.PropertyType;
                if (type.IsValueType || type == typeof(string) || type == typeof(byte[]))
                    info.SetValue(model, info.GetValue(newModel));
            }
        }


        public static void CopyEntity<TEntity>(this TEntity entity, TEntity newObject)
        {
            var keyName = typeof(TEntity).GetPrimaryKey().Name;
            foreach (var info in typeof(TEntity).GetProperties().Where(t => t.Name != keyName))
            {
                var value = info.GetValue(newObject);
                var type = info.PropertyType;
                if (value != null && type.IsValueType || type == typeof(string)) 
                    info.SetValue(entity, value);
            }
        }

        public static object GetMyValue(this object obj, string strName, bool checkNull = true)
        {
            if (checkNull == false && obj == null)
                return null;
            object result = obj;
            var type = result.GetType();
            foreach (var str in strName.Split('.'))
            {
                var temp = type.GetProperty(str);
                result = temp.GetValue(result);
                if (result == null)
                {
                    var type1 = temp.PropertyType;
                    if (!checkNull)
                        return null;
                    else
                        if (!type1.IsNullableType() && type1 != typeof(string) && type1 != typeof(byte[]))
                            throw new CaspianException(null, 1);
                }
                type = temp.PropertyType;
            }
            return result;
        }
    }
}
