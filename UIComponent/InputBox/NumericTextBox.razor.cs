using Caspian.Common;
using Microsoft.JSInterop;
using Caspian.Common.Extension;
using Microsoft.AspNetCore.Components;

namespace Caspian.UI
{
    public partial class NumericTextBox<TValue>: CBaseInput<TValue>
    {
        [Parameter]
        public int Total { get; set; } = 8;

        [Parameter]
        public int? NumberDigit { get; set; } = 2;

        [Parameter]
        public bool DigitGrouping { get; set; }

        string GetDigitGrouping(string digit)
        {
            if (!digit.HasValue())
                return "";
            var array = digit.Split('.');
            var str = Convert.ToInt64(array[0]).DigitGrouping();
            if (array.Length > 1)
                str += '.' + array[1];
            return str;
        }

        Dictionary<string, object> GetAttributes()
        {
            var attributes = new Dictionary<string, object>();
            var className = "t-widget t-textbox";
            if (disabled)
                className += " t-state-disabled";
            if (!disabled && ErrorMessage.HasValue())
            {
                className += " t-state-error";
                attributes["error-message"] = ErrorMessage;
            }
            attributes["class"] = className;
            attributes["total"] = Total;
            if (DigitGrouping)
                attributes["digit-grouping"] = true;
            if (NumberDigit.HasValue)
                attributes["number-digit"] = NumberDigit.Value;
            if (Style.HasValue())
                attributes["style"] = Style;

            return attributes;
        }

        async Task OnChangeValue(ChangeEventArgs arg)
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
