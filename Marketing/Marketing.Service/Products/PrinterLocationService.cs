using Caspian.Common;
using Marketing.Model;
using Caspian.Common.Service;

namespace Marketing.Service
{
    public class PrinterLocationService: BaseService<PrinterLocation>, IBaseService<PrinterLocation>
    {
        public PrinterLocationService(IServiceProvider provider) : base(provider)
        {
            RuleFor(t => t.Name).Required().UniqueAsync("محل چاپی با این عنوان در سیستم ثبت شده است");
        }
    }
}
