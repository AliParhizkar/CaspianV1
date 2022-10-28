using Demo.Model;
using Caspian.Common;
using Caspian.Common.Service;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace Demo.Service
{
    public class CityService : SimpleService<City>, ISimpleService<City>
    {
        public CityService(IServiceScope scope)
            : base(scope)
        {
            RuleFor(t => t.Title).Required().UniqAsync(t => t.Province, "شهری با این عنوان در این استان تعریف شده است.");
        }
    }
}
