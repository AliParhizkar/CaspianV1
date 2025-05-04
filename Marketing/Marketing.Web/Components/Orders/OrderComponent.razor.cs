using Caspian.UI;
using Caspian.Common;
using Marketing.Model;
using Marketing.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Marketing.Web.OrderComponents
{
    public partial class OrderComponent: ComponentBase
    {
        WindowStatus statusLevel1, statusLevel2;
        double Level2Top;
        int? selectedOrderId;
        IList<Order> orders;

        void OpenLevel1(MouseEventArgs e)
        {
            statusLevel1 = WindowStatus.Open;
        }

        void OpenLevel2(MouseEventArgs e, int orderId)
        {
            Console.WriteLine(orderId);
            Level2Top = e.ClientY - e.OffsetY;
            statusLevel2 = WindowStatus.Open;
            selectedOrderId = orderId;
        }

        protected override async Task OnInitializedAsync()
        {
            using var service = Page.CreateScope().GetService<OrderService>();
            orders = await service.GetAll().Where(t => t.SettleType == null).ToListAsync();
            await base.OnInitializedAsync();
        }

        [Parameter]
        public EventCallback<int> OnChange { get; set; }

        [Parameter]
        public BasePage Page { get; set; }
    }
}
