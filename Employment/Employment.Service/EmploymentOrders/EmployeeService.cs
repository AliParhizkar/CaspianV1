using Employment.Model;
using Caspian.Common.Service;
using Microsoft.Extensions.DependencyInjection;

namespace Employment.Service
{
    public class EmployeeService : BaseService<Employee>, IBaseService<Employee>
    {
        public EmployeeService(IServiceProvider provider) :
            base(provider)
        {

        }
    }
}
