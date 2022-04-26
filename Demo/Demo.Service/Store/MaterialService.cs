using System;
using Demo.Model;
using System.Linq;
using Caspian.Common;
using Caspian.Common.Service;
using Microsoft.Extensions.DependencyInjection;

namespace Demo.Service
{
    public class MaterialService : SimpleService<Material>, ISimpleService<Material>
    {
        public MaterialService(IServiceScope scope)
            : base(scope)
        {
            RuleFor(t => t.Title).Required().UniqAsync("محصولی با این عنوان تعریف شده است");
        }
    }
}
