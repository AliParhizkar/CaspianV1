using Caspian.Common;
using Marketing.Model;
using Caspian.Common.Service;

namespace Marketing.Service
{
    public class ProductCategoryService : BaseService<ProductCategory>, IBaseService<ProductCategory>
    {
        public ProductCategoryService(IServiceProvider provider)
            : base(provider)
        {
            RuleFor(t => t.Name).Required().UniqueAsync("گروه محصولی با این عنوان در سیستم ثبت شده است");
            RuleFor(t => t.Code).UniqueAsync("گروه محصولی با این کد در سیستم ثبت شده است")
            .CustomValue(code =>
            {
                if (!code.HasValue())
                    return false;
                return code.Length > 2;
            }, "کد گروه محصول باید بیش از دو کاراکتر باشد");
        }
    }
}
