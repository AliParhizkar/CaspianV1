using Demo.Model;
using System.Linq;
using Caspian.Engine;
using Caspian.Common;
using Caspian.Common.Service;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Demo.Service
{
    [ReportClass]
    public class ProductService : SimpleService<Product>, ISimpleService<Product>
    {
        public ProductService(IServiceScope scope)
            : base(scope)
        {
            RuleFor(t => t.Title).Required().UniqAsync("محصولی با این عنوان تعریف شده است");
            RuleFor(t => t.Price).CustomValue(t => t < 0, "مبلغ کالا نمی تواند منفی باشد.");
            RuleFor(t => t.PriceOuterBound).CustomValue(t => t < 0, "مبلغ بیرون بر نمی تواند منفی باشد.");
            RuleFor(t => t.Meal).CustomValue(t => t == 0, "حداقل یکی از وعده های غذایی باید انتخاب شوند");
            RuleFor(t => t.Code).UniqAsync("محصولی با این کد در سیستم وجود دارد")
                .Custom(p => 
                {
                    if (!p.Code.HasValue())
                        return false;
                    return new ProductCategoryService(ServiceScope).GetAll().Any(pc => pc.Code == p.Code);
                }, "گروه محصولی با این کد در سیستم ثبت شده است");
        }


        public async Task UpdatePrice(int id, int price)
        {
            var old = await SingleAsync(id);
            old.Price = price;
            await UpdateAsync(old);
        }

        public async Task UpdateActiveType(int id, bool value)
        {
            var old = await SingleAsync(id);
            old.ActiveType = value ? ActiveType.Enable : ActiveType.Disable;
            await UpdateAsync(old);
        }

        public async Task UpdatePriceOuterBound(int id, int priceOuterBound)
        {
            var old = await SingleAsync(id);
            old.PriceOuterBound = priceOuterBound;
            await UpdateAsync(old);
        }

        public async Task ToggleEnable(int id)
        {
            var old = await SingleAsync(id);
            old.ActiveType = old.ActiveType == ActiveType.Enable ? old.ActiveType = ActiveType.Disable : old.ActiveType = ActiveType.Enable;
            await UpdateAsync(old);
        }

        public async Task ToggleOutofstock(int id)
        {
            var old = await SingleAsync(id);
            old.OutofStock = !old.OutofStock;
            await UpdateAsync(old);
        }

        [ReportMethod("محصولات")]
        public IQueryable<Product> GetReportProducts(Product product)
        {
            return GetAll(product);
        }

        [Task("بررسی محصول")]
        public bool CheckProduct(Product product)
        {
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        [Task("بررسی نوع مشتری")]
        public CustomerType CheckAction(Product product)
        {
            return CustomerType.Real;
        }

        [Task("پردازش محصول")]
        public void ProcessProduct(Product product)
        {

        }
    }
}
