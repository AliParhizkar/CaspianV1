using Caspian.Common;
using Marketing.Model;
using Caspian.Common.Service;

namespace Marketing.Service
{
    public class ProductDescriptionService : BaseService<ProductDescription>, IBaseService<ProductDescription>
    {
        public ProductDescriptionService(IServiceProvider provider)
            :base(provider) 
        {
            RuleFor(t => t.Description).UniqueAsync(t => t.ProductId, "این عنوان برای محصول ثبت شده است");
        }
    }
}
