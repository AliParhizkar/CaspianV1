using Procurement.Model;
using Caspian.Common.Service;

namespace Procurement.Service
{
    public class SupplierService:BaseService<Supplier>
    {
        public SupplierService(IServiceProvider provider)
            :base(provider)
        {

        }
    }
}
