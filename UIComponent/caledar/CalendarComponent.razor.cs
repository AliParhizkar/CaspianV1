using Caspian.Common;
using Microsoft.JSInterop;
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
        int[] monthConvertor = new int[] { 4, 8, 12, 3, 7, 11, 2, 6, 10, 1, 5, 9 };
        //PersianDate pDate;

        [Parameter]
        public DateTime Date { get; set; }

        [Parameter]
        public EventCallback<DateTime> DateChanged { get; set; }

        [Parameter]
        public bool PersianCalendar { get; set; }

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
            var pDate = date.ToPersianDate();
            var year = PersianCalendar ? pDate.Year.Value : date.Year;
            switch (viewType)
            {
                case ViewType.Month:
                    var month = PersianCalendar ? pDate.Month.ConvertToInt().Value : date.Month;
                    headerTitle = months[month - 1] + " " + year;
                    break;
                case ViewType.Year:
                    headerTitle = year.ToString();
                    break;
                case ViewType.Decade:
                    var startYear = year / 10 *10;
                    headerTitle = $"{startYear}-{startYear + 10}";
                    break;
                case ViewType.Century:
                    var startDecade = year / 100 * 100;
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
                    index = PersianCalendar ? monthConvertor[date.ToPersianDate().Month.ConvertToInt().Value - 1] - 1 : date.Month - 1;
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
            if (PersianCalendar)
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
            await JSRuntime.InvokeVoidAsync("caspian.DatePicker.bindCalendar", element, viewType, vNavigation);
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
