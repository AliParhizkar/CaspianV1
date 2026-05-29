using Caspian.Common;
using System.Reflection;
using Microsoft.JSInterop;
using System.Linq.Expressions;
using Caspian.Common.Extension;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Components;

namespace Caspian.UI
{
    public partial class NumericTextBox<TValue>: CBaseInput<TValue>
    {
        int numberDigit, total;

        [Parameter]
        public int? Total { get; set; }

        [Parameter]
        public int? NumberDigit { get; set; }

        [Parameter]
        public bool DigitGrouping { get; set; }

        [CascadingParameter(Name = "ParentControlIsValueSearch")]
        public bool ParentControlIsValueSearch { get; set; }

        protected override void OnInitialized()
        {
            if (Id.HasValue())
            {
                InputAttributes["id"] = Id.Replace('.', '_');
                InputAttributes["name"] = Id.Replace('.', '_');
            }
            var type = typeof(TValue).GetUnderlyingType();
            PrecisionAttribute attribute = null;
            if (ValueExpression != null)
            {
                var member = (ValueExpression.Body as MemberExpression).Member;
                attribute = member.GetCustomAttribute<PrecisionAttribute>();
            }
            if (type == typeof(byte) || type == typeof(short) || type == typeof(int) || type == typeof(long))
                numberDigit = 0;
            else if (NumberDigit == null)
                numberDigit = attribute?.Scale ?? 2;
            else
                numberDigit = NumberDigit.Value;
            if (Total == null)
                total = attribute?.Precision ?? 10;
            else
                total = Total.Value;
            base.OnInitialized();
        }

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
            attributes["total"] = total;
            if (DigitGrouping)
                attributes["digit-grouping"] = true;
            if (numberDigit > 0)
                attributes["number-digit"] = numberDigit;
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
            BindEditContext();
            if (ValueChanged.HasDelegate)
                await ValueChanged.InvokeAsync(Value);
            if (OnChange.HasDelegate)
                await OnChange.InvokeAsync(Value);
        }

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender && (EntitySearch == null || ParentControlIsValueSearch))
                await jsRuntime.InvokeVoidAsync("caspian.common.bindTextBox", InputElement);
            await base.OnAfterRenderAsync(firstRender);
        }
    }
}
