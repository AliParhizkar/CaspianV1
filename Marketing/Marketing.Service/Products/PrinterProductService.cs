using Caspian.Common;
using Marketing.Model;
using Caspian.Common.Service;

namespace Marketing.Service
{
    public class PrinterProductService: BaseService<PrinterProduct>, IBaseService<PrinterProduct>
    {
        public PrinterProductService(IServiceProvider provider) : base(provider)
        {
            RuleFor(t => t.ProductId).UniqueAsync(t => t.PrinterId, "این محصول قبلا به این محل چاپ تخصیص داده شده است.");
        }
    }
}
