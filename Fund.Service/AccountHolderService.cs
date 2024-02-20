using Caspian.Common;
using Caspian.Common.Service;
using Fund.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fund.Service
{
    public class AccountHolderService : BaseService<AccountHolder>, IBaseService<AccountHolder>
    {
        public AccountHolderService(IServiceProvider provider)
            : base(provider)
        {
            RuleFor(t => t.UserId).Required();
            RuleFor(t => t.Status).Required();
            RuleFor(t => t.BeginDate).Required();
            RuleFor(t => t.EndDate).Required();
        }
    }
}
