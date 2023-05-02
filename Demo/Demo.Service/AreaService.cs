using System;
using Demo.Model;
using Caspian.Common;
using Caspian.Common.Service;

namespace Demo.Service
{
    public class AreaService : BaseService<Area>, IBaseService<Area>
    {
        public AreaService(IServiceProvider provider)
            : base(provider)
        {
            RuleFor(t => t.Title).Required().UniqAsync(t => t.CityId, "منطقه ای با این عنوان در این شهر تعریف شده است.");
        }
    }
}
