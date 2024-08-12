using Caspian.Common;
using System.Reflection;
using System.Linq.Expressions;
using Caspian.Common.Extension;
using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;

namespace Caspian.UI
{
    public partial class RadioList<TValue> : ComponentBase, IRadioList
    {
        string className;
        Dictionary<TValue, string> dic;
        async void UpdateValue(TValue value)
        {
            Value = value;
            if (ValueChanged.HasDelegate)
                await ValueChanged.InvokeAsync(value);
            if (OnChange.HasDelegate)
                await OnChange.InvokeAsync();
        }

        [Parameter]
        public EventCallback OnChange { get; set; }

        [Parameter]
        public DefaultLayout DefaultLayout { get; set; } = DefaultLayout.SpaceBetween;

        [Parameter]
        public string ClassName { get; set; }

        public object GetValue()
        {
            return Value;
        }

        public async Task ChangeValueAsync(object value)
        {
            Value = (TValue)Convert.ChangeType(value, typeof(TValue).GetUnderlyingType());
            if (ValueChanged.HasDelegate)
                await ValueChanged.InvokeAsync(Value);
            if (OnChange.HasDelegate)
                await OnChange.InvokeAsync(Value);
        }

        protected override void OnInitialized()
        {
            if (RadioItems == null)
            {
                var type = typeof(TValue).GetUnderlyingType();
                if (type.IsEnum)
                {
                    dic = new Dictionary<TValue, string>();
                    foreach (FieldInfo fi in type.GetFields().Where(t => !t.IsSpecialName).OrderBy(t => t.GetValue(null)))
                    {
                        var da = fi.GetCustomAttribute<DisplayAttribute>();
                        if (da != null)
                            dic.Add((TValue)fi.GetValue(null), da.Name);
                        else
                            dic.Add((TValue)fi.GetValue(null), fi.Name);
                    }
                }
            }
            base.OnInitialized();
        }

        [Parameter]
        public EventCallback OnChangeValue { get; set; }

        [Parameter]
        public bool Disabled { get; set; }


        [Parameter]
        public string Style { get; set; }

        [Parameter]
        public TValue Value { get; set; }

        [Parameter]
        public EventCallback<TValue> ValueChanged { get; set; }

        [Parameter]
        public Expression<Func<TValue>> ValueExpression { get; set; }

        [Parameter]
        public IList<SelectListItem> Source { get; set; }

        [Parameter]
        public RenderFragment RadioItems { get; set; }

        protected override void OnParametersSet()
        {
            className = "justify-content-" + DefaultLayout.GetCssClassName();
            if (ClassName.HasValue())
                className += " " + ClassName;
            base.OnParametersSet();
        }

    }
}
