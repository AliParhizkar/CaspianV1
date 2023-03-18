using System.Threading.Tasks;

namespace Caspian.UI
{
    public partial class CalendarComponent
    {
        HNavigation? hNavigation;

        async Task NavigateLTR()
        {
            hNavigation = HNavigation.LeftToRight;
            switch(viewType) 
            {
                case ViewType.Month:
                    date = date.AddMonths(-1);
                    break;
                case ViewType.Year:
                    date = date.AddYears(-1);
                    break;
                case ViewType.Decade: 
                    selectedDecade--; 
                    break;
                case ViewType.Century:
                    selectedDecade -= 10;
                    break;
            }
            await Task.Delay(400);
            hNavigation = null;
        }

        async Task NavigateRTL()
        {
            hNavigation = HNavigation.RightToLeft;
            switch (viewType)
            {
                case ViewType.Month:
                    date = date.AddMonths(1);
                    break;
                case ViewType.Year:
                    date = date.AddYears(1);
                    break;
                case ViewType.Decade:
                    selectedDecade++;
                    break;
                case ViewType.Century:
                    selectedDecade += 10;
                    break;
            }
            await Task.Delay(400);
            hNavigation = null;
        }
    }
}
