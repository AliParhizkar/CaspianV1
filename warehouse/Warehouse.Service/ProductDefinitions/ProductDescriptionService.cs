using Caspian.Common;
using Warehouse.Model;
using Caspian.Common.Service;

namespace Warehouse.Service
{
    //public class ProductDescriptionService : BaseService<ProductDescription>
    //{
    //    public ProductDescriptionService(IServiceProvider provider)
    //        :base(provider)
    //    {
    //        RuleFor(t => t.Name).Required().UniqueAsync(t => t.ProductId, "برای این محصول مشخص ای با این عنوان تعریف شده است");
    //        RuleFor(t => t.Value).Required().Custom(t =>
    //        {
    //            if (t.DescriptionType == DescriptionType.Numerical)
    //            {
    //                if (!int.TryParse(t.Value, out _))
    //                    return $"مقدار {t.Value} برای فرمت عددی نامعتبر می باشد";
    //            }
    //            else if (t.DescriptionType == DescriptionType.Date)
    //            {
    //                if (!DateOnly.TryParse(t.Value, out _))
    //                    return $"مقدار {t.Value} برای فرمت تاریخ نامعتبر می باشد";
    //            }
    //            return null;
    //        });
    //    }
    //}
}
