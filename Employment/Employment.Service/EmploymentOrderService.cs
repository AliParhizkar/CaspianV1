using Caspian.Engine;
using Employment.Model;
using Caspian.Common.Service;
using Microsoft.Extensions.DependencyInjection;

namespace Employment.Service
{
    [ReportClass]
    public class EmploymentOrderService : SimpleService<EmploymentOrder>, ISimpleService<EmploymentOrder>
    {
        public EmploymentOrderService(IServiceScope scope)
            : base(scope)
        {

        }

        [ReportMethod("حکم غیرهیات علمی")]
        public IQueryable<EmploymentOrder> GetEmploymentOrders(EmploymentOrder order)
        {
            return GetAll(order);
        }
    }
}
