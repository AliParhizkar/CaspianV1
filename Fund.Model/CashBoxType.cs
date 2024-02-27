using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fund.Model
{
    public class CashBoxType
    {
        [Key]
        public int Id { get; set; }

        public long Code { get; set; }

        public string Name { get; set; }

        public string InternationalName { get; set; }

        public string Description { get; set; }

        [Display(Name= "Currency")]
        public int CurrencyId { get; set; }

        [ForeignKey(nameof(CurrencyId))]
        public virtual Currency Currency { get; set; }
    }
}