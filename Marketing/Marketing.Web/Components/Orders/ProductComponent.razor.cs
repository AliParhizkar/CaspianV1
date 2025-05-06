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
        WindowStatus productStatus;
        IList<ProductCategory> categories;
        int? categoryId, subCategoryId, selectedProductId, left, top;
        FactorComponent factor;

        protected override async Task OnInitializedAsync()
        {
            using var scope = Page.CreateScope();
            var service = scope.GetService<ProductCategoryService>();
            categories = (await service.GetAll().ToListAsync()).Where(t => t.CategoryId == null).ToList();
            await base.OnInitializedAsync();

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

        async Task GetProducts(int categoryId)
        {
            using var service = Page.CreateScope().GetService<ProductService>();
            products = await service.GetAll().Where(t => t.CategoryId == categoryId).ToListAsync();
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

        public IList<OrderDetailTopping> DeletedToppings => factor.GetDeletedToppings();

        public IList<OrderDetailTopping> ChangedToppings => factor.GetToppingsChange();


        [Parameter]
        public BatchService<Order, OrderDetail> OrderService { get; set; }

        [Parameter]
        public Customer Customer { get; set; }

        [Parameter]
        public BasePage Page { get; set; }

        [Parameter]
        public EventCallback<IList<OrderDetail>> OnChange { get; set; }

        [Parameter]
        public IList<Product> Products { get; set; }
    }
}
