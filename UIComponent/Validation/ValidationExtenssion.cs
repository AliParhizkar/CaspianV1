using Microsoft.AspNetCore.Components.Forms;

namespace Caspian.UI
{
    public static class ValidationExtenssion
    {
        public static void SetModel(this EditContext context, object model)
        {
            foreach (var info in context.Model.GetType().GetProperties())
            {
                var type = info.PropertyType;
                if (type.IsValueType || type == typeof(string) || type == typeof(byte[]))
                {
                    var newValue = info.GetValue(model);
                    var oldValue = info.GetValue(context.Model);
                    if (newValue != null && !newValue.Equals(oldValue) || 
                        oldValue != null && !oldValue.Equals(newValue))
                    {
                        info.SetValue(context.Model, newValue);
                        //context.NotifyFieldChanged(new FieldIdentifier(context.Model, info.Name));
                    }
                }
            }
        }
    }
}
