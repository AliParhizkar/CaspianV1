using Caspian.Common;
using Marketing.Model;
using Caspian.Common.Service;

namespace Marketing.Service
{
    public class ProductToppingService : BaseService<ProductTopping>, IBaseService<ProductTopping>
    {
        public ProductToppingService(IServiceProvider provider)
            :base(provider)
        {
            RuleFor(t => t.ProductId).UniqueAsync(t => t.ToppingId, "این تاپینگ قبلا به محصول اضافه شده است");
        }
    }
}
