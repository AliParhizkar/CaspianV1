using Caspian.UI;
using Caspian.Common;
using Marketing.Model;
using Marketing.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Components;

namespace Marketing.Web.OrderComponents
{
    public partial class FactorComponent
    {
        IDictionary<int, UIService<OrderDetail, OrderDetailTopping>> toppingServices;
        UIService<OrderDetail, OrderDetailTopping> toppingService;
        WindowStatus toppingStatus, descriptStatus;
        int? selectedDetailId, toppingProductId;
        double left, top;
        OrderDetail selectedOrderDetail;

        async Task ChangeDescription(WindowStatus status)
        {
            await OrderService.DetailDataView.UpdateAsync(selectedOrderDetail);
            descriptStatus = status; 
            selectedOrderDetail = null;
        }

        async Task ToppingChanged(decimal sum)
        {
            var toppings = OrderService.DetailDataView.GetBatchEntities();
            var old = toppings.Single(t => t.Id == selectedDetailId.Value);
            old.ToppingAmount = sum;
            /// Initialize Topping To Validate
            old.OrderDetailToppings = toppingService.DetailDataView.GetBatchEntities().ToList();
            await OrderService.DetailDataView.UpdateAsync(old);
            var data = OrderService.DetailDataView.GetBatchEntities();
            await OnChange.InvokeAsync(data);
        }

        public void ClearToppingService()
        {
            toppingServices.Clear();
        }

        async Task UpdateQuantity(OrderDetail orderDetail, decimal value)
        {
            if (value >= 0)
            {
                var grid = OrderService.DetailDataView;
                var old = grid.GetBatchEntities().SingleOrDefault(t => t.ProductId == orderDetail.ProductId);
                if (value == 0)
                {
                    /// Count of toppings that inserted for order-detail
                    var toppingsCount = 0;
                    if (orderDetail.Id > 0)
                    {
                        using var service = Page.CreateScope().GetService<OrderDetailToppingService>();
                        toppingsCount += await service.GetAll().Where(t => t.OrderDetailId == orderDetail.Id).CountAsync();
                    }
                    /// Count of toppings added - Count of toppings that inserted
                    var toppingService = toppingServices.SingleOrDefault(t => t.Key == orderDetail.Id).Value;
                    if (toppingService != null)
                    {
                        /// Toppings count added
                        toppingsCount += toppingService.ChangedEntities.Count(t => t.ChangeStatus == ChangeStatus.Added);
                        toppingsCount -= toppingService.ChangedEntities.Count(t => t.ChangeStatus == ChangeStatus.Deleted);
                    }
                    if (toppingsCount > 0)
                        Page.ShowMessage("محصول دارای تاپینگ می باشد لطفا ابتدا تاپینگ های محصول را حذف کنید.");
                    else
                        await grid.RemoveAsync(old);
                }
                else
                {
                    old.Quantity = (decimal)value;
                    await grid.UpdateAsync(old);
                }
                await OnChange.InvokeAsync();
            }
            else
                Page.ShowMessage("تعداد نمی تواند منفی باشد");
        }

        protected override void OnInitialized()
        {
            toppingServices = new Dictionary<int, UIService<OrderDetail, OrderDetailTopping>>();
            base.OnInitialized();
        }

        public IList<OrderDetailTopping> GetDeletedToppings()
        {
            var list = new List<OrderDetailTopping>();
            foreach (var service in toppingServices)
            {
                foreach (var detail in service.Value.ChangedEntities)
                {
                    if (detail.ChangeStatus == ChangeStatus.Deleted)
                        list.Add(detail.Entity);
                }
            }
            return list;
        }

        public IList<OrderDetailTopping> GetOrderDetailToppings(int detailId)
        {
            return null;
        }

        public IList<OrderDetailTopping> GetToppingsChange()
        {
            var list = new List<OrderDetailTopping>();
            foreach (var service in toppingServices)
            {
                foreach (var detail in service.Value.ChangedEntities)
                {
                    if (detail.ChangeStatus != ChangeStatus.Deleted)
                    {
                        detail.Entity.OrderDetailId = service.Key;
                        list.Add(detail.Entity);
                    }
                }
            }
            return list;
        }

        public async Task AddToOrder(Product product)
        {
            var grid = OrderService.DetailDataView;
            var old = grid.GetBatchEntities().SingleOrDefault(t => t.ProductId == product.Id);
           
            if (old == null)
            {
                var detail = new OrderDetail()
                {
                    OrderId = OrderService.MasterId,
                    ProductId = product.Id,
                    Quantity = 1,
                };
                UpdatePriceAndDiscount(detail);
                await grid.InsertAsync(detail);
            }
            else
            {
                old.Quantity++;
                await grid.UpdateAsync(old);
            }
            await OnChange.InvokeAsync();
            await grid.ScrollIntoViewSelectedRow();
        }

        protected override void OnParametersSet()
        {
            if (OrderService.DetailDataView != null)
            {
                foreach (var detail in OrderService.DetailDataView.GetBatchEntities())
                    UpdatePriceAndDiscount(detail);
            }
            base.OnParametersSet();
        }

        void UpdatePriceAndDiscount(OrderDetail detail)
        {
            var oldProduct = Products.Single(t => t.Id == detail.ProductId);
            if (IsSpecialCustomer == true)
            {
                detail.Price = oldProduct.SpecialCustomerPrice.GetValueOrDefault();
                detail.Discount = 0;
            }
            else
            {
                detail.Discount = oldProduct.Discount;
                if (OrderType == OrderType.Salon)
                    detail.Price = oldProduct.Price;
                else
                    detail.Price = oldProduct.TakeOutPrice;
            }
        }

        void OpenDescription(EMouseEventArgs e, OrderDetail detail)
        {
            left = e.ClientX - e.OffsetX + e.Width + 5;
            top = e.ClientY - e.OffsetY - 100;
            if (top < 10)
                top = 10;
            selectedOrderDetail = detail;
            descriptStatus = WindowStatus.Open;
        }

        void OpenTopping(EMouseEventArgs e, OrderDetail detail)
        {
            left = e.ClientX - e.OffsetX + e.Width + 15;
            top = e.ClientY - e.OffsetY - 100;
            if (top < 10)
                top = 10;
            selectedDetailId = detail.Id;
            toppingService = toppingServices.SingleOrDefault(t => t.Key == detail.Id).Value;
            if (toppingService == null)
            {
                toppingService = new UIService<OrderDetail, OrderDetailTopping>(OrderService.ServiceProvider);
                toppingService.MasterId = detail.Id;
                toppingServices.Add(detail.Id, toppingService);
            }
            toppingStatus = WindowStatus.Open;
            toppingProductId = detail.ProductId;
        }

        [Parameter]
        public bool? IsSpecialCustomer { get; set; }

        [Parameter]
        public IList<Product> Products { get; set; }

        [Parameter]
        public OrderType OrderType { get; set; }

        [Parameter]
        public EventCallback OnChange { get; set; }

        [Parameter]
        public BasePage Page { get; set; }

        [Parameter]
        public UIService<Order, OrderDetail> OrderService { get; set; }
    }
}
