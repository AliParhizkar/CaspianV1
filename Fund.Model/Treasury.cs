using Caspian.Common;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fund.Model
{
    [Display(Name = "خزانه")]
    public class Treasury : Account
    {
    }

    public class TreasuryValidator : AbstractValidator<Treasury>
    {
        public TreasuryValidator()
        {
            RuleFor(r => r.CashBoxType).Required();
            RuleFor(r => r.AccountHolder).Required();
            RuleFor(r => r.Title).Required();
            RuleFor(r => r.FloorLimit).Required();
            RuleFor(r => r.Status).Required();
        }
    }
}
