using Caspian.Common;
using Marketing.Model;
using Caspian.Common.Service;

namespace Marketing.Service
{
    public class ProductService : BaseService<Product>, IBaseService<Product>
    {
        public ProductService(IServiceProvider provider)
            : base(provider)
        {
            RuleFor(t => t.Title).Required().UniqueAsync("محصولی  با این عنوان در سیستم وجود دارد.");
            RuleFor(t => t.Code).UniqueAsync("محصولی با این کد در سیستم وجود دارد")
            .CustomValue(code =>
            {
                if (!code.HasValue())
                    return false;
                return code.Length > 2;
            }, "کد باید بیش از دو کاراکتر باشد");
            RuleFor(t => t.CategoryId).CustomAsync(async t =>
            {
                return await provider.GetCaspianService<ProductCategoryService>().AnyAsync(u => u.CategoryId == t.CategoryId);
            }, "گروه محصول دارای زیرگروه می باشد و نمی تواند دارای محصول باشد.");
        }
    }
}
