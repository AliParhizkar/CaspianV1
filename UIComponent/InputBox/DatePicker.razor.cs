using Caspian.Common;
using Caspian.Common.Extension;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Caspian.UI
{
    public partial class DatePicker<TValue> : CBaseInput<TValue> 
    {
        WindowStatus Status;
        Dictionary<string, object> attrs;
        string text;
        ElementReference element;
        bool isDateOnly;

        void OpenWindow()
        {
            if (!disabled)
                Status = WindowStatus.Open;
        }

        async Task ChangeDate(DateOnly date)
        {
            if (!disabled)
            {
                var type = typeof(TValue).GetUnderlyingType();
                if (type == typeof(DateTime))
                    Value = (TValue)Convert.ChangeType(date, type);
                else
                {
                    var dateOnly = new DateOnly(date.Year, date.Month, date.Day);
                    Value = (TValue)Convert.ChangeType(dateOnly, type);
                }
                SetDateString(date);
                if (ValueChanged.HasDelegate)
                    await ValueChanged.InvokeAsync(Value);
                Status = WindowStatus.Close;
            }
        }

        async void ChangeValue(ChangeEventArgs arg)
        {
            if (!disabled)
            {
                Value = default;
                var strValue = Convert.ToString(arg.Value);
                if (strValue.HasValue())
                {
                    if (typeof(TValue).GetUnderlyingType() == typeof(DateTime))
                        Value = (TValue)Convert.ChangeType(strValue, typeof(DateTime));
                    else
                    {
                        var value = (DateTime)Convert.ChangeType(strValue, typeof(DateTime));
                        Value = (TValue)(object)value.ToDateOnly();
                    }

                }
                if (DefaultMode)
                    text = strValue;
                await ValueChanged.InvokeAsync(Value);
            }
        }

        [CascadingParameter(Name = "ParentControlIsValueSearch")]
        public bool ParentControlIsValueSearch { get; set; }

        [Parameter]
        public bool PersianCalendar { get; set; } = true;

        [Parameter]
        public bool OpenOnFocus { get; set; }

        [Parameter]
        public DateOnly? FromDate { get; set; }

        [Parameter]
        public DateOnly? ToDate { get; set; }

        [Parameter]
        public bool DefaultMode { get; set; }

        protected override void OnInitialized()
        {
            attrs = new Dictionary<string, object>();
            isDateOnly = typeof(TValue).GetUnderlyingType() == typeof(DateOnly);
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

            if (Value == null || Value.Equals(default(TValue)))
                text = "";
            else
            {
                DateTime date = default;
                if (isDateOnly)
                {
                    var dateOnly = (DateOnly)Convert.ChangeType(Value, typeof(DateOnly));
                    date = dateOnly.ToDateTime(new TimeOnly());
                }
                else
                    date = Convert.ToDateTime(Value);
                SetDateString(date.ToDateOnly());
            }
            base.OnParametersSet();
        }

        void SetDateString(DateOnly? date)
        {
            if (date == null)
                text = string.Empty;
            else if (DefaultMode)
                text = date.ConvertToBrowserDate();
            else if (PersianCalendar)
                text = date.ToPersianDateString();
            else
                text = date.ToShortDateString();
        }

        async protected override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender && (EntitySearch == null || ParentControlIsValueSearch))
            {
                var dotnet = DotNetObjectReference.Create(this);
                await jsRuntime.InvokeVoidAsync("caspian.common.bindDatePicker", element, dotnet);
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
