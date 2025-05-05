using Caspian.UI;
using Caspian.Common;
using Marketing.Model;
using Marketing.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Marketing.Web.OrderComponents
{
    public partial class ProductComponent
    {
        IList<Product> products;
        WindowStatus productStatus, toppingStatus;
        IList<ProductCategory> categories;
        int? categoryId, subCategoryId, selectedProductId, left, top, toppingProductId;
        IDictionary<int, BatchService<OrderDetail, OrderDetailTopping>> toppingServices;
        BatchService<OrderDetail, OrderDetailTopping> toppingService;
        IDictionary<int, double> sumToppings;
        int? selectedDetailId;

        async Task ToppingChanged(decimal sum)
        {
            var old = OrderService.DetailDataView.GetBatchEntities().Single(t => t.Id == selectedDetailId.Value);
            old.ToppingAmount = sum;
            await OrderService.DetailDataView.UpdateAsync(old);
        }

        protected override void OnInitialized()
        {
            toppingServices = new Dictionary<int, BatchService<OrderDetail, OrderDetailTopping>>();
            base.OnInitialized();
        }

        void OpenTopping(int detailId, int productId)
        {
            selectedDetailId = detailId;
            toppingService = toppingServices.SingleOrDefault(t => t.Key == detailId).Value;
            if (toppingService == null)
            {
                toppingService = new BatchService<OrderDetail, OrderDetailTopping>(OrderService.Provider);
                toppingService.MasterId = detailId;
                toppingServices.Add(detailId, toppingService);
            }
            toppingStatus = WindowStatus.Open;
            toppingProductId = productId;
        }

        protected override async Task OnInitializedAsync()
        {
            using var scope = Page.CreateScope();
            var service = scope.GetService<ProductCategoryService>();
            categories = (await service.GetAll().ToListAsync()).Where(t => t.CategoryId == null).ToList();
            if (OrderService.MasterId > 0)
            {
                var detailService = scope.GetService<OrderDetailToppingService>();

                var result = await detailService.GetAll().Where(t => t.OrderDetail.OrderId == OrderService.MasterId).GroupBy(t => t.OrderDetailId).Select(t => new
                {
                    OrderDeatilId = t.Key,
                    Sum = t.Sum(u => u.PriceTotal)
                }).ToListAsync();
                sumToppings = result.ToDictionary(t => t.OrderDeatilId, t => (double)t.Sum);
            }
            await base.OnInitializedAsync();

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
                await CallbackForChange();
            }
            else
                Page.ShowMessage("تعداد نمی تواند منفی باشد");
        }

        async Task AddToOrder(Product product)
        {
            var grid = OrderService.DetailDataView;
            var old = grid.GetBatchEntities().SingleOrDefault(t => t.ProductId == product.Id);
            if (old == null)
            {
                await grid.InsertAsync(new OrderDetail()
                {
                    OrderId = OrderService.MasterId,
                    ProductId = product.Id,
                    Quantity = 1,
                    Price = product.Price,
                    Discount = product.Discount
                });
            }
            else
            {
                old.Quantity++;
                await grid.UpdateAsync(old);
            }
            await CallbackForChange();
            await grid.ScrollIntoViewSelectedRow();
        }

        async Task CallbackForChange()
        {
            var data = OrderService.DetailDataView.GetBatchEntities();
            await OnChange.InvokeAsync(data);
        }

        async Task SelectCategory(ProductCategory category)
        {
            subCategoryId = category.Id;
            productStatus = WindowStatus.Close;
            await GetProducts(category.Id);
        }

        async Task GetProducts(int productId)
        {
            using var service = Page.CreateScope().GetService<ProductService>();
            products = await service.GetAll().Where(t => t.CategoryId == productId).ToListAsync();
        }

        async Task SelectCategory(MouseEventArgs args, ProductCategory category)
        {
            categoryId = category.Id;
            if (category.Categories != null)
            {
                productStatus = WindowStatus.Open;
                left = (int)(args.PageX - args.OffsetX - 205);
                top = (int)(args.PageY - args.OffsetY);
            }
            else
            {
                subCategoryId = category.Id;
                await GetProducts(category.Id);
            }
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

        [Parameter]
        public BatchService<Order, OrderDetail> OrderService { get; set; }

        [Parameter]
        public BasePage Page { get; set; }

        [Parameter]
        public EventCallback<IList<OrderDetail>> OnChange { get; set; }
    }
}
