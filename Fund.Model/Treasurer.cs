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

    public enum Status : byte
    {
        Enable = 1,
        Disable,
    }
}
