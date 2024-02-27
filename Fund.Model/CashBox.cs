using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fund.Model
{
    public class CashBox : Account 
    {
        [Display(Name = "نوع Teller")]
        public TellerTypeEnum? TellerType {  get; set; }
    }

    public enum TellerTypeEnum : byte
    {
        User = 1,
        ATM
    }
}