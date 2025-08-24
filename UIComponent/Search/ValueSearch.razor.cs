using Caspian.Common;
using System.Reflection;
using System.ComponentModel;
using System.Linq.Expressions;
using Caspian.Common.Extension;
using Microsoft.AspNetCore.Components;

namespace Caspian.UI
{
    public partial class ValueSearch
    {
        string propertyPath, title;
        ValueTypeControl valueTypeControl;

        [Parameter]
        public bool Wrap { get; set; }

        [Parameter]
        public Expression ValueExpression { get; set; }

        [CascadingParameter]
        internal IEntitySearch EntitySearch { get; set; }

        [CascadingParameter]
        internal PageData PageData { get; set; }

        [Parameter]
        public int? TotalSpan { get; set; }

        [Parameter]
        public int? ColSpan { get; set; }

        string GetLabelCSSClassName()
        {
            if (TotalSpan.HasValue)
            {
                var className = PageData?.RightToLeft == true ? "pe-2" : "ps-2";
                className += " col-md-";
                return className + (TotalSpan.Value - ColSpan);
            }
            return (EntitySearch as ICaspianContainer).GetLabelContainerCSSClassName(ColSpan.Value);
        }

        string GetControlCSSClassName()
        {
            var str = $"col-md-{ColSpan} ";
            return str + (PageData?.RightToLeft == true ? "ps-2" : "pe-2");
        }

        async Task SearchByValue(bool? value)
        {
            EntitySearch.SetValue(propertyPath, value);
            await EntitySearch.SearchAsync();
        }

        async Task SearchByFromValue(object value)
        {
            EntitySearch.SetFromValue(propertyPath, value);
            await EntitySearch.SearchAsync();
        }

        async Task SearchByFromValue(DateOnly? value)
        {
            EntitySearch.SetFromValue(propertyPath, value);
            await EntitySearch.SearchAsync();
        }

        async Task SearchByToValue(object value)
        {
            EntitySearch.SetToValue(propertyPath, value);
            await EntitySearch.SearchAsync();
        }

        async Task SearchByToValue(DateOnly? value)
        {
            EntitySearch.SetToValue(propertyPath, value);
            await EntitySearch.SearchAsync();
        }

        protected override void OnInitialized()
        {
            if (ValueExpression != null && EntitySearch != null)
            {
                propertyPath = (ValueExpression as MemberExpression).GetPropertyPath();
                var member = (ValueExpression as MemberExpression).Member as PropertyInfo;
                var type = member.PropertyType.GetUnderlyingType();
                if (type == typeof(DateTime) || type == typeof(DateOnly))
                    valueTypeControl = ValueTypeControl.Date;
                else if (type == typeof(TimeOnly))
                    valueTypeControl = ValueTypeControl.Time;
                else if (type == typeof(bool))
                    valueTypeControl = ValueTypeControl.Boolean;
                else
                    valueTypeControl = ValueTypeControl.Numeric;
                title = member.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? member.Name;
            }
            base.OnInitialized();
        }
    }
}
