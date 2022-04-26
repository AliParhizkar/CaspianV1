using System;
using System.Linq;
using Caspian.Common;
using Newtonsoft.Json;
using System.Reflection;
using Microsoft.JSInterop;
using System.Threading.Tasks;
using Caspian.Common.Attributes;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace Caspian.UI
{
    public partial class MultiSelectBox<TValue>: CBaseInput<TValue> 
    {
        IDictionary<TValue, string> Items;

        [Parameter]
        public TValue DisabledValue { get; set; }

        async Task ChangValue(TValue value, bool flag)
        {
            if (flag)
                Value = (dynamic)Value | (dynamic)value;
            else
            {
                var tempValue = typeof(TValue).GetField(value.ToString()).GetValue(null);
                Value = (TValue)((dynamic)Value - (dynamic)tempValue);
            }
            if (ValueChanged.HasDelegate)
                await ValueChanged.InvokeAsync(Value);
        }

        protected override void OnInitialized()
        {
            var fields = typeof(TValue).GetFields().Where(t => !t.IsSpecialName);
            TabIndex = 0;
            var index = 0;
            Items = new Dictionary<TValue, string>();
            foreach (var field in fields)
            {
                var value = field.GetValue(null);
                if (Convert.ToInt64(value) != Math.Pow(2, index))
                    throw new CaspianException("In type " + typeof(TValue).Name + " field " + field.Name + " value is invalid");
                var attr = field.GetCustomAttribute<EnumFieldAttribute>();
                var title = attr == null ? field.Name : attr.DisplayName;
                Items.Add((TValue)value, title);
                index++;
            }
            base.OnInitialized();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            var json = JsonConvert.SerializeObject(new
            {
                errorMessage = ErrorMessage,
            });
            await jsRuntime.InvokeVoidAsync("$.telerik.bindMultiSelect", htmlElement, json);
            await base.OnAfterRenderAsync(firstRender);
        }
    }
}
