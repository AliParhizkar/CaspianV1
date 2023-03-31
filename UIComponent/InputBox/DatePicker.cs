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
        WindowStatus Status;
        Dictionary<string, object> attrs;
        string text;
        ElementReference element;

        void OpenWindow()
        {
            if (!Disabled)
                Status = WindowStatus.Open;
        }

        async Task ChangeDate(DateTime date)
        {
            if (!Disabled)
            {
                if (typeof(TValue) == typeof(DateTime) || typeof(TValue) == typeof(DateTime?))
                    Value = (TValue)Convert.ChangeType(date, typeof(DateTime));
                else
                    Value = (TValue)Convert.ChangeType(date.ToPersianDate(), typeof(PersianDate));
                text = date.Date.ToPersianDateString();
                await Task.Delay(400);
                Status = WindowStatus.Close;
            }
        }

        async void ChangeValue(ChangeEventArgs arg)
        {
            if (!Disabled)
            {
                TValue value = default;
                var strValue = Convert.ToString(arg.Value);

                if (!strValue.Contains("_"))
                    value = (TValue)Convert.ChangeType(new PersianDate(strValue).GetMiladyDate().Value, typeof(DateTime));
                Value = value;
                await ValueChanged.InvokeAsync(value);
            }
        }

        [Parameter]
        public bool OpenOnFocus { get; set; }

        [Parameter]
        public DateTime? FromDate { get; set; }

        [Parameter]
        public DateTime? ToDate { get; set; }

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
            if (OpenOnFocus)
            {
                attrs["onfocus"] = EventCallback.Factory.Create(this, () =>
                {
                    Status = WindowStatus.Open;
                });
            }
            else
                attrs.Remove("onfocus");
            if (Value == null || Convert.ToDateTime(Value) == default(DateTime))
                text = "";
            else
                text = Convert.ToDateTime(Value).Date.ToPersianDateString();
            base.OnParametersSet();
        }

        async protected override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                var dotnet = DotNetObjectReference.Create(this);
                await jsRuntime.InvokeVoidAsync("$.caspian.bindDatePicker", dotnet, element);
            }
            await base.OnAfterRenderAsync(firstRender);
        }

        [JSInvokable]
        public void CloseWindow()
        {
            Status = WindowStatus.Close;
            StateHasChanged();
        }
    }
}
