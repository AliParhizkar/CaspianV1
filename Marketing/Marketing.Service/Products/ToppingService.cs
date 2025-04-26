using Caspian.Common;
using Marketing.Model;
using Caspian.Common.Service;

namespace Marketing.Service
{
    public class ToppingService : BaseService<Topping>, IBaseService<Topping>
    {
        public ToppingService(IServiceProvider provider)
            :base(provider)
        {
            RuleFor(t => t.Name).Required().UniqueAsync("محصولی با این عنوان در سیستم تعریف شده است");
            RuleFor(t => t.Price).CustomValue(t => t < 0, "قیمت نمی تواند منفی باشد.");
        }
    }
}
