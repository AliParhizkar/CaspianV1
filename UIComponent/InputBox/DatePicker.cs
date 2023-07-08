﻿using System;
using Caspian.Common;
using System.Threading;
using Microsoft.JSInterop;
using System.Globalization;
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

        void UpdateValue()
        {

        }

        async Task ChangeDate(DateTime date)
        {
            if (!Disabled)
            {
                Value = (TValue)Convert.ChangeType(date, typeof(DateTime));
                text = date.ToShortDateString();
                if (ValueChanged.HasDelegate)
                    await ValueChanged.InvokeAsync(Value);
                await Task.Delay(400);
                Status = WindowStatus.Close;
            }
        }

        async void ChangeValue(ChangeEventArgs arg)
        {
            if (!Disabled)
            {
                Value = default;
                var strValue = Convert.ToString(arg.Value);
                if (strValue.HasValue())
                    Value = (TValue)Convert.ChangeType(strValue, typeof(DateTime));
                if (DefaultMode)
                    text = strValue;
                await ValueChanged.InvokeAsync(Value);
            }
        }

        [Parameter]
        public bool PersianCalendar { get; set; }

        [Parameter]
        public bool OpenOnFocus { get; set; }

        [Parameter]
        public DateTime? FromDate { get; set; }

        [Parameter]
        public DateTime? ToDate { get; set; }

        [Parameter]
        public bool DefaultMode { get; set; }

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
            {
                var date = Convert.ToDateTime(Value);
                if (DefaultMode)
                    text = date.ConvertToBrowserDate();
                else
                    text = date.ToShortDateString();

            }
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
