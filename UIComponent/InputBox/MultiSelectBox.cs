using System;
using System.Linq;
using Caspian.Common;
using System.Reflection;
using Microsoft.JSInterop;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

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

        [Parameter]
        public DefaultLayout DefaultLayout { get; set; } = DefaultLayout.SpaceBetween;

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
                var attr = field.GetCustomAttribute<DisplayAttribute>();
                var title = attr == null ? field.Name : attr.Name;
                Items.Add((TValue)value, title);
                index++;
            }
            base.OnInitialized();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            var json = JsonSerializer.Serialize(new
            {
                errorMessage = ErrorMessage,
            });
            await jsRuntime.InvokeVoidAsync("$.caspian.bindMultiSelect", htmlElement, json);
            await base.OnAfterRenderAsync(firstRender);
        }
    }
}
