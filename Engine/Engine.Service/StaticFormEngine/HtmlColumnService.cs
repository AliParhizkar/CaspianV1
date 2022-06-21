﻿using Caspian.Common.Service;
using Microsoft.Extensions.DependencyInjection;

namespace Caspian.Engine.Service
{
    public class HtmlColumnService : SimpleService<HtmlColumn>, ISimpleService<HtmlColumn>
    {
        public HtmlColumnService(IServiceScope scope)
            :base(scope)
        {

        }
    }
}