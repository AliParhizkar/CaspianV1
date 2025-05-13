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

        async Task SelectOrder(Order order)
        {
            statusLevel1 = statusLevel2 = WindowStatus.Close;
            await OnChange.InvokeAsync(order.Id);
        }

        async Task OpenLevel1(MouseEventArgs e)
        {
            using var service = Page.CreateScope().GetService<OrderService>();
            var date = DateTime.Now.ToDateOnly();
            orders = await service.GetAll().Where(t => !t.IsSettled && t.OrderDate == date).ToListAsync();
            statusLevel1 = WindowStatus.Open;
        }

        void OpenLevel2(MouseEventArgs e, int orderId)
        {
            Level2Top = e.ClientY - e.OffsetY;
            statusLevel2 = WindowStatus.Open;
            selectedOrderId = orderId;
        }


        [Parameter]
        public EventCallback<int> OnChange { get; set; }

        [Parameter]
        public BasePage Page { get; set; }
    }
}
