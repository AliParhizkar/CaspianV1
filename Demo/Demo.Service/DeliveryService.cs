using System;
using Demo.Model;
using Caspian.Common;
using Caspian.Common.Service;

namespace Demo.Service
{
    public class DeliveryService : BaseService<Delivery>, IBaseService<Delivery>
    {
        public DeliveryService(IServiceProvider provider)
            : base(provider)
        {
            RuleFor(t => t.LName).Required();
            RuleFor(t => t.Code).UniqAsync("پیکی با این کد پرسنلی در سیستم وجود دارد.");
        }
    }
}
