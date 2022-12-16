using Employment.Model;
using Caspian.Common.Service;
using Microsoft.Extensions.DependencyInjection;

namespace Employment.Service
{
    public class EmployeeService : SimpleService<Employee>, ISimpleService<Employee>
    {
        public EmployeeService(IServiceScope scope) :
            base(scope)
        {

        }
    }
}
