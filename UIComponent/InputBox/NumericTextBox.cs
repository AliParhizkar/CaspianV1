using System;
using Caspian.Common;
using Newtonsoft.Json;
using System.Threading;
using Microsoft.JSInterop;
using System.Threading.Tasks;
using Caspian.Common.Extension;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace Caspian.UI
{
    public partial class NumericTextBox<TValue>: CBaseInput<TValue>
    {
        string oldJson;
        int? maxLength = 8;

        [Parameter, JsonProperty("total")]
        public int Total { get; set; }

        [Parameter, JsonProperty("digits")]
        public int NumberDigit { get; set; } = 2;

        [Parameter, JsonProperty("group")]
        public bool DigitGrouping { get; set; } = true;

        async Task onChangeValue(ChangeEventArgs arg)
        {
            var str = arg.Value.ToString().Replace(",", "");
            if (str == "-")
                str = "";
            if (str.HasValue())
            {
                var decimalDigits = Convert.ToChar(Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator);
                str = str.Replace('.', decimalDigits);
                Value = (TValue)Convert.ChangeType(str, typeof(TValue).GetUnderlyingType());
            }
            else
                Value = default;
            if (ValueChanged.HasDelegate)
                await ValueChanged.InvokeAsync(Value);
            if (OnChange.HasDelegate)
                await OnChange.InvokeAsync(Value);
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
            if (NumberDigit == 0)
                maxLength = Total;
            else
                maxLength = null;
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
                Focused = false;
                oldJson = json;
                await jsRuntime.InvokeVoidAsync("$.caspian.bindControl", htmlElement, json, UiControlType.TextBox);
            }
            await base.OnAfterRenderAsync(firstRender);
        }
    }
}
