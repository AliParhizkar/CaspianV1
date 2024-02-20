using Caspian.Common;
using Caspian.Engine.Model;
using FluentValidation;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fund.Model
{
    [Display(Name = "خزانه دار")]
    public class Treasurer : AccountHolder
    {
    }

    public enum Status
    {
        Enable = 1,
        Disable,
    }

    public class TreasurerValidator : AbstractValidator<Treasurer>
    {
        public TreasurerValidator()
        {
            RuleFor(r => r.User).Required();
            RuleFor(r => r.Status).Required();
            RuleFor(r => r.BeginDate).Required();
            RuleFor(r => r.EndDate).Required();
        }
    }
}
