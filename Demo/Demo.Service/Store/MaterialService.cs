﻿using System;
using Demo.Model;
using Caspian.Common;
using Caspian.Common.Service;

namespace Demo.Service
{
    public class MaterialService : BaseService<Material>, IBaseService<Material>
    {
        public MaterialService(IServiceProvider provider)
            : base(provider)
        {
            RuleFor(t => t.Title).Required().UniqAsync("محصولی با این عنوان تعریف شده است");
        }
    }
}
