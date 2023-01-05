﻿using Caspian.Common;
using Employment.Model;
using Caspian.Common.Service;

namespace Employment.Service
{
    public class OccupationService : SimpleService<Occupation>, ISimpleService<Occupation>
    {
        public OccupationService(IServiceProvider provider)
            : base(provider)
        {
            RuleFor(t => t.Title).Required().UniqAsync("رسته شغلی با این عنوان در سیستم وجود دارد");
        }
    }
}