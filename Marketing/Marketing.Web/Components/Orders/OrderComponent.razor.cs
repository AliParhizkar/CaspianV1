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
        WindowStatus statusLevel2;
        double Level2Top;
        int? selectedOrderId;
        IList<Order> orders;

        async Task SelectOrder(Order order) => await OnChange.InvokeAsync(order.Id);

        protected override async Task OnInitializedAsync()
        {
            using var service = Page.CreateScope().GetService<OrderService>();
            var date = DateTime.Now.ToDateOnly();
            orders = await service.GetAll().Where(t => !t.IsSettled && t.OrderDate == date).ToListAsync();
            await base.OnInitializedAsync();
        }

        [Parameter]
        public WindowStatus Status { get; set; }

        [Parameter]
        public EventCallback<WindowStatus> StatusChanged { get; set; }

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
