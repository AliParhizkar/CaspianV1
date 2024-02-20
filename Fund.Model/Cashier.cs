using Caspian.Common;
using Caspian.Engine.Model;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fund.Model
{
    [Display(Name = "صندوق دار")]
    public class Cashier : AccountHolder
    {

    }

    public class CashierValidator : AbstractValidator<Cashier>
    {
        public CashierValidator()
        {
            RuleFor(r => r.User).Required();
            RuleFor(r => r.Status).Required();
            RuleFor(r => r.BeginDate).Required();
            RuleFor(r => r.EndDate).Required();
        }
    }
}
