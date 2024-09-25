using Microsoft.AspNetCore.Components;

namespace Caspian.UI
{
    public partial class DayHours
    {
        double[] itemsTop = new double[] 
        { 
            5, 45.718, 75, 115, 155, 184.282, 195, 184.282, 155, 115, 75, 45.718,
            35, 19.7372, 60, 115, 170, 210.263, 225, 210.263, 170, 115, 60, 19.7372
        };

        double[] itemsLeft = new double[] 
        { 
            115, 155, 184.282, 195, 184.282, 155, 115, 75, 45.718, 35, 45.718, 75,
            115, 170, 210, 225, 210, 170, 115, 60, 19.7372, 5, 19.7372, 60
        };

        async Task HourSelected(int hour)
        {
            var enable = true;
            if (FromHour.HasValue && hour < FromHour.Value)
                enable = false;
            if (ToHour.HasValue && hour > ToHour.Value)
                enable = false;
            if (enable && SelectedHourChanged.HasDelegate)
                await SelectedHourChanged.InvokeAsync(hour);
        }

        [Parameter]
        public int? FromHour { get; set; }

        [Parameter]
        public int? ToHour { get; set; }

        [Parameter]
        public int SelectedHour { get; set; }

        [Parameter]
        public EventCallback<int> SelectedHourChanged { get; set; }
    }
}
