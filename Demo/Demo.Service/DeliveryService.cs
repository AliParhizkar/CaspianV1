using System;
using Demo.Model;
using Caspian.Common;
using Caspian.Common.Service;

namespace Demo.Service
{
    public class DeliveryService : BaseService<Courier>
    {
        public DeliveryService(IServiceProvider provider)
            : base(provider)
        {
            RuleFor(t => t.LName).Required();
            RuleFor(t => t.Code).UniqueAsync("Code should be uniq.");
        }
    }
}
