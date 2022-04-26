using System;
using Caspian.Common;
using Microsoft.JSInterop;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace Caspian.UI
{
    public partial class DatePicker<TValue> : CBaseInput<TValue>
    {
        Dictionary<string, object> attrs;
        string text;
        async void ChangeValue(ChangeEventArgs arg)
        {
            TValue value = default;
            var strValue = Convert.ToString(arg.Value);
            
            if (!strValue.Contains("_"))
                value = (TValue)Convert.ChangeType(new PersianDate(strValue).GetMiladyDate().Value, typeof(DateTime));
            Value = value;
            await ValueChanged.InvokeAsync(value);
        }

        protected override void OnInitialized()
        {
            attrs = new Dictionary<string, object>();
            base.OnInitialized();
        }

        protected override void OnParametersSet()
        {
            if (Id.HasValue())
            {
                attrs.Add("id", Id.Replace('.', '_'));
                attrs.Add("name", Id.Replace('.', '_'));
            }
            if (Value == null)
                text = "";
            else
                text = Convert.ToDateTime(Value).Date.ToPersianDateString();
            base.OnParametersSet();
        }

        async protected override Task OnAfterRenderAsync(bool firstRender)
        {
            var json = this.ConvertToJson();
            await jsRuntime.InvokeVoidAsync("$.telerik.bindControl", htmlElement, json, UiControlType.DatePicker);
            await base.OnAfterRenderAsync(firstRender);
        }
    }
}
