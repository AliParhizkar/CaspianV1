using Caspian.Common;
using Microsoft.JSInterop;
using Caspian.Common.Extension;
using Microsoft.AspNetCore.Components;
using System.Text.Json;

namespace Caspian.UI
{
    public partial class NumericTextBox<TValue>: CBaseInput<TValue>
    {
        int? maxLength = 8;

        [Parameter]
        public int Total { get; set; } = 8;

        [Parameter]
        public int? NumberDigit { get; set; } = 2;

        [Parameter]
        public bool DigitGrouping { get; set; } = true;

        Dictionary<string, object> GetAttributes()
        {
            var attributes = new Dictionary<string, object>();
            var className = "t-widget t-numerictextbox";
            if (disabled)
                className += " t-state-disabled";
            if (!disabled && ErrorMessage != null)
                className += " t-state-error";
            attributes["class"] = className;
            attributes["total"] = Total;
            if (NumberDigit.HasValue)
                attributes["number-digit"] = NumberDigit.Value;
            if (Style.HasValue())
                attributes["style"] = Style;

            return attributes;
        }

        async Task onChangeValue(ChangeEventArgs arg)
        {
            var str = arg.Value.ToString().Replace(",", "");
            if (str == "-" || str == ".-" || str == "-.")
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
            if (firstRender)
                await jsRuntime.InvokeVoidAsync("caspian.common.bindTextBox", InputElement);
            await base.OnAfterRenderAsync(firstRender);
        }
    }
}
