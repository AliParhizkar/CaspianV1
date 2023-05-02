using Caspian.Engine;
using Employment.Model;
using Caspian.Common.Service;
using Microsoft.Extensions.DependencyInjection;

namespace Employment.Service
{
    [ReportClass]
    public class ParametricEmploymentOrderService : BaseService<ParametricEmploymentOrder>, IBaseService<ParametricEmploymentOrder>
    {
        public ParametricEmploymentOrderService(IServiceProvider provider)
            : base(provider)
        {

        }

        [ReportMethod("حکم غیرهیات علمی")]
        public IQueryable<ParametricEmploymentOrder> GetEmploymentOrders(ParametricEmploymentOrder order)
        {
            return GetAll(order);
        }

        [ReportMethod("برای تست")]
        public IQueryable<ForTestEmployment> GetForTestEmployments(ForTestEmployment forTest)
        {
            return null;
        }
    }
}
