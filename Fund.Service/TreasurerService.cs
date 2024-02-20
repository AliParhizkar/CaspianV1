using Caspian.Common;
using Caspian.Common.Service;
using Fund.Model;

namespace Fund.Service
{
    public class TreasurerService :  BaseService<Treasurer>, IBaseService<Treasurer>
    {

        public override Task<Treasurer> AddAsync(Treasurer entity)
        {
            return base.AddAsync(entity);
        }

        public TreasurerService(IServiceProvider provider)
            : base(provider)
        {
            RuleFor(t => t.UserId).Required();
            RuleFor(t => t.Status).Required();
            RuleFor(t => t.BeginDate).Required();
            RuleFor(t => t.EndDate).Required();
        }
    }
}