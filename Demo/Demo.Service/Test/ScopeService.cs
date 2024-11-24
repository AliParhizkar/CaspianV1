using System;
using Demo.Model;
using Caspian.Common.Service;
using Caspian.Common;

namespace Demo.Service
{
    public class ScopeService : BaseService<Scope>, IBaseService<Scope>
    {
        public ScopeService(IServiceProvider provider)
            : base(provider)
        {
            RuleFor(t => t.Title).Required().UniqAsync("حوزه ای با این عنوان در سیستم تعریف شده است");
        }
    }
}
