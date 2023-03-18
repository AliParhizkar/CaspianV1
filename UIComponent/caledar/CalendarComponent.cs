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
        ViewType viewType = ViewType.Month;
        ElementReference element;
        string[] months;
        DateTime date = DateTime.Now;
        //int selectedYear;
        int selectedDecade;
        //DecadeComponent decadeComponent;
        //CentuaryComponent centuaryComponent;

        [Parameter]
        public DateTime Date { get; set; }

        async Task NavigateUp()
        {
            if (viewType < ViewType.Century)
            {
                vNavigation = VNavigation.Up;
                viewType++;
                await Task.Delay(400);
                vNavigation = null;
            }
        }

        async Task NavigateDown(int data)
        {
            vNavigation = VNavigation.Down;
            switch (viewType)
            {
                case ViewType.Year:
                    date = date.ChangeMonth(data);
                    break;
                case ViewType.Decade:
                    date = date.ChangeYear(data);
                    break;
                case ViewType.Century:
                    selectedDecade = data;
                    break;
            }
            viewType--;
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
            selectedDecade = date.Year / 10;
            base.OnParametersSet();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await JSRuntime.InvokeVoidAsync("$.caspian.bindDatePicker", element, viewType, vNavigation);
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

    public enum ViewType
    {
        Month = 1,

        Year,

        Decade,

        Century
    }
}
