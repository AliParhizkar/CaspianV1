using System;
using Caspian.Common;
using Newtonsoft.Json;
using Microsoft.JSInterop;
using System.Threading.Tasks;
using Caspian.Common.Extension;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using System.Linq.Expressions;

namespace Caspian.UI
{
    public partial class NumericTextBox<TValue>: CBaseInput<TValue>
    {
        string oldJson;

        [Parameter, JsonProperty("total")]
        public int Total { get; set; }

        [Parameter, JsonProperty("digits")]
        public int NumberDigit { get; set; } = 2;

        [Parameter, JsonProperty("group")]
        public bool DigitGrouping { get; set; } = true;

        async Task onChangeValue(ChangeEventArgs arg)
        {
            var str = arg.Value.ToString().Replace(",", "");
            if (str.HasValue())
                Value = (TValue)Convert.ChangeType(str, typeof(TValue).GetUnderlyingType());
            else
                Value = default;
            if (ValueChanged.HasDelegate)
                await ValueChanged.InvokeAsync(Value);
            if (InputAttributes != null && InputAttributes.ContainsKey("onchange"))
            {
                var oncChange = (EventCallback<ChangeEventArgs>)InputAttributes["onchange"];
                await oncChange.InvokeAsync(arg);
            }
        }

        protected override void OnInitialized()
        {
            InputAttributes = new Dictionary<string, object>();
            base.OnInitialized();
        }

        protected override void OnParametersSet()
        {
            if (Id.HasValue())
            {
                InputAttributes["id"] = Id.Replace('.', '_');
                InputAttributes["name"] = Id.Replace('.', '_');
            }
            base.OnParametersSet();
        }

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            var type = typeof(TValue);
            if (type.IsNullableType())
                type = Nullable.GetUnderlyingType(type);
            if (type == typeof(int) || type == typeof(long) || type == typeof(short))
                NumberDigit = 0;
            var json = this.ConvertToJson();
            
            if (!json.Equals(oldJson))
            {
                oldJson = json;
                await jsRuntime.InvokeVoidAsync("$.telerik.bindControl", htmlElement, json, UiControlType.TextBox);
            }
            await base.OnAfterRenderAsync(firstRender);
        }
    }
}
