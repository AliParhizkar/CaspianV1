using Caspian.Common.Service;
using Microsoft.Extensions.DependencyInjection;

namespace Caspian.Engine.Service
{
    public class FormGroupService : SimpleService<FormGroup>
    {
        public FormGroupService(IServiceScope scope)
            :base(scope)
        {

        }
    }
}
