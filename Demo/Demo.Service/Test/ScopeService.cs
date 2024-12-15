using System;
using Demo.Model;
using Caspian.Common;
using Caspian.Common.Service;

namespace Demo.Service
{
    public class ScopeService : MasterDetailsService<Scope, Evaluation>, IBaseService<Scope>
    {
        public ScopeService(IServiceProvider provider)
            : base(provider)
        {
            RuleFor(t => t.Title).Required().UniqueAsync("حوزه ای با این عنوان در سیستم تعریف شده است");
            RuleForEach(t => t.Evaluations).SetValidator(t => new EvaluationService(provider));
        }
    }
}
