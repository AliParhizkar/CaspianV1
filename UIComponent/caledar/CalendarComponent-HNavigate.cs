using System.Threading.Tasks;

namespace Caspian.UI
{
    public partial class CalendarComponent
    {
        HNavigation? hNavigation;

        async Task NavigateLTR()
        {
            hNavigation = HNavigation.LeftToRight;
            switch(index) 
            {
                case 1:
                    date = date.AddMonths(1);
                    selectedMonth= date.Month;
                    selectedYear= date.Year;
                    break;
            }
            await Task.Delay(400);
            hNavigation = null;
        }

        async Task NavigateRTL()
        {
            hNavigation = HNavigation.RightToLeft;
        }
    }
}
