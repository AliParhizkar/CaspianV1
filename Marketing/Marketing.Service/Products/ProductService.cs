using Caspian.Common;
using Marketing.Model;
using Caspian.Common.Service;

namespace Marketing.Service
{
    public class ProductService : MasterDetailsService<Product, ProductTopping, ProductDescription>, IBaseService<Product>
    {
        public ProductService(IServiceProvider provider)
            : base(provider)
        {
            FillDetail = false;
            RuleFor(t => t.Title).Required().UniqueAsync("محصولی  با این عنوان در سیستم وجود دارد.");
            RuleFor(t => t.Code).UniqueAsync("محصولی با این کد در سیستم وجود دارد")
            .CustomValue(code =>
            {
                if (!code.HasValue())
                    return false;
                return code.Length > 2;
            }, "کد باید بیش از دو کاراکتر باشد");
            RuleFor(t => t.Price).CustomValue(t => t < 0, "قیمت محصول نمی تواند منفی باشد");
            RuleFor(t => t.Discount).Custom(t => t.Discount > t.Price, "تخفیف نمی تواند از قیمت محصول بیشتر باشد")
                .CustomValue(t => t < 0, "تخفیف نمی تواند منفی باشد");
            RuleFor(t => t.CategoryId).CustomAsync(async t =>
            {
                return await provider.GetCaspianService<ProductCategoryService>().AnyAsync(u => u.CategoryId == t.CategoryId);
            }, "گروه محصول دارای زیرگروه می باشد و نمی تواند دارای محصول باشد.");
            RuleForEach(t => t.ProductToppings).SetValidator(new ProductToppingService(provider));
            RuleForEach(t => t.ProductDescriptions).SetValidator(new ProductDescriptionService(provider));
        }
    }
}
