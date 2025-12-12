using Caspian.Common;
using Microsoft.JSInterop;
using Caspian.Common.Extension;
using Microsoft.AspNetCore.Components;

namespace Caspian.UI
{
    public partial class NumericTextBox<TValue>: CBaseInput<TValue>
    {
        int numberDigit;

        [Parameter]
        public int Total { get; set; } = 8;

        [Parameter]
        public int? NumberDigit { get; set; } = 2;

        [Parameter]
        public bool DigitGrouping { get; set; }

        [CascadingParameter(Name = "ParentControlIsValueSearch")]
        public bool ParentControlIsValueSearch { get; set; }

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

        protected override void OnParametersSet()
        {
            if (Id.HasValue())
            {
                InputAttributes["id"] = Id.Replace('.', '_');
                InputAttributes["name"] = Id.Replace('.', '_');
            }
            var type = typeof(TValue).GetUnderlyingType();
            if (type == typeof(byte) || type == typeof(short) || type == typeof(int) || type == typeof(long))
                numberDigit = 0;
            else
                numberDigit = NumberDigit ?? 2;
            base.OnParametersSet();
        }

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender && (EntitySearch == null || ParentControlIsValueSearch))
            {
                var regular = numberDigit > 0 ? "" : "^-?\\d{0," + Total + "}$";
                await jsRuntime.InvokeVoidAsync("caspian.common.bindTextBox", InputElement, regular);
            }
            await base.OnAfterRenderAsync(firstRender);
        }
    }
}
