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
        string navigateDownClassName;
        string headerTitle;

        [Parameter]
        public DateTime Date { get; set; }

        [Parameter]
        public EventCallback<DateTime> DateChanged { get; set; }

        [Parameter]
        public DateTime? FromDate { get; set; }

        [Parameter]
        public DateTime? ToDate { get; set; }

        async Task ChangeDate(DateTime dateTime)
        {
            Date = dateTime;
            await DateChanged.InvokeAsync(dateTime);

        }

        async Task NavigateUp()
        {
            if (viewType < ViewType.Century)
            {
                vNavigation = VNavigation.Up;
                viewType++;
                CalendarTitleInit();
                await Task.Delay(400);
                vNavigation = null;
            }
        }

        void CalendarTitleInit()
        {
            switch (viewType)
            {
                case ViewType.Month:
                    headerTitle = months[date.Month - 1] + " " + date.Year;
                    break;
                case ViewType.Year:
                    headerTitle = date.Year.ToString();
                    break;
                case ViewType.Decade:
                    var startYear = date.Year / 10 *10;
                    headerTitle = $"{startYear}-{startYear + 10}";
                    break;
                case ViewType.Century:
                    var startDecade = date.Year / 100 * 100;
                    headerTitle = $"{startDecade}-{startDecade + 100}";
                    break;
            }
        }

        void ClassNameInit()
        {
            int index = 0;
            switch (viewType)
            {
                case ViewType.Month:
                    index = date.Month - 1;
                    break;
                case ViewType.Year:
                    index = date.Year % 10 + 1;
                    break;
                case ViewType.Decade:
                    index = (date.Year / 10) % 10 + 1;
                    break;
            }
            navigateDownClassName = $"c-down-to-state c-left-{index % 4} c-top-{index / 4}";
        }

        async Task NavigateDown(DateTime dateTime)
        {
            vNavigation = VNavigation.Down;
            date = dateTime;
            viewType--;
            ClassNameInit();
            CalendarTitleInit();
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
            date = Date;
            ClassNameInit();
            CalendarTitleInit();
            base.OnParametersSet();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await JSRuntime.InvokeVoidAsync("$.caspian.bindCalendar", element, viewType, vNavigation);
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

    /// <summary>
    /// This enum should start from 1 and order of fields should not changed
    /// </summary>
    public enum ViewType
    {
        Month = 1,

        Year,

        Decade,

        Century
    }
}
