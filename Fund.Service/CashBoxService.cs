using Caspian.Common;
using Caspian.Common.Service;
using FluentValidation;
using Fund.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fund.Service
{
    public class CashBoxService : BaseService<CashBox>, IBaseService<CashBox>
    {
        public CashBoxService(IServiceProvider provider)
            : base(provider)
        {
            RuleFor(t => t.CashBoxTypeId).Required();
            RuleFor(t => t.AccountHolderId).Required();
            RuleFor(t => t.Title).MaximumLength(50).Required();
            RuleFor(t => t.FloorLimit).Required();
            RuleFor(t => t.Status).Required();
            RuleFor(t => t.TellerType).Required();
        }
    }
}