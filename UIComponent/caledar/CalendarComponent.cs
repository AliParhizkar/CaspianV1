using System;
using Caspian.Common;
using Microsoft.JSInterop;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Caspian.UI
{
    public partial class CalendarComponent
    {
        VNavigation? vNavigation;
        int index = 1;
        ElementReference element;
        string[] months;
        DateTime date = DateTime.Now;
        int selectedMonth;
        int selectedYear;
        int selectedDecade;
        DecadeComponent decadeComponent;
        CentuaryComponent centuaryComponent;

        [Parameter]
        public DateTime Date { get; set; }

        async Task NavigateUp()
        {
            if (index < 4)
            {
                vNavigation = VNavigation.Up;
                index++;
                await Task.Delay(400);
                vNavigation = null;
            }
        }

        async Task NavigateDown(int data)
        {
            vNavigation = VNavigation.Down;
            switch (index)
            {
                case 1:

                    break;
                case 2:
                    index = 1;
                    selectedMonth = data;
                    break;
                case 3:
                    index = 2;
                    selectedYear = data;
                    break;
                case 4:
                    index = 3;
                    selectedDecade = data;
                    break;
            }
            await Task.Delay(400);
            vNavigation = null;
        }

        protected override void OnInitialized()
        {
            service.Language = Language.En;
            if (service.Language == Language.Fa)
                months = new string[] { "فروردین", "اردیبهشت", "خرداد", "تیر", "مرداد", "شهریور", "مهر", "آبان", "آذر", "دی", "بهمن", "اسفند" };
            else
                months = new string[] { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
            base.OnInitialized();
        }

        protected override void OnParametersSet()
        {
            selectedMonth = date.Month;
            selectedYear = date.Year;
            selectedDecade = date.Year / 10;
            base.OnParametersSet();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await JSRuntime.InvokeVoidAsync("$.caspian.bindDatePicker", element, index, vNavigation);
            await base.OnAfterRenderAsync(firstRender);
        }
    }

    public enum VNavigation
    {
        Up = 1,
        
        Down
    }

    public enum HNavigation
    {
        LeftToRight = 1,

        RightToLeft,
    }
}
