using Microsoft.EntityFrameworkCore.Query.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Model
{
    [Table("FinancialUnits", Schema = "acc")]
    public class FinancialUnit
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("عنوان")]
        public string Title { get; set; }

        [DisplayName("تصویر")]
        public byte[] Image { get; set; }

        [DisplayName("توضیحات"), MaxLength(200)]
        public string Description { get; set; }
    }
}
