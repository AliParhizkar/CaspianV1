using Caspian.Common;
using Warehouse.Model;
using Caspian.Common.Service;

namespace Warehouse.Service
{
    //public class ProductService: MasterDetailsService<Product, ProductDescription>, IMasterDetailsService<Product, ProductDescription>
    //{
    //    public ProductService(IServiceProvider provider)
    //        :base(provider)
    //    {
    //        RuleFor(t => t.Name).Required().UniqueAsync("کالایی با این نام در سیستم تعریف شده است");
    //        RuleFor(t => t.Code).UniqueAsync("کالایی با این کد در سیستم تعریف شده است");

    //        RuleForEach(t => t.ProductDescriptions).SetValidator(new ProductDescriptionService(provider));
    //    }
    //}
}
