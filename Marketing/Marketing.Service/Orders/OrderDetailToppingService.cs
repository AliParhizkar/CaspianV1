using Caspian.Common;
using Marketing.Model;
using Caspian.Common.Service;

namespace Marketing.Service
{
    public class OrderDetailToppingService : BaseService<OrderDetailTopping>, IBaseService<OrderDetailTopping>
    {
        public OrderDetailToppingService(IServiceProvider serviceProvider) 
            : base(serviceProvider) 
        {
            RuleFor(t => t.Price).CustomValue(t => t < 0, "قیمت نمی تواند منفی باشد");
        }
    }
}
