using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Procurement.Model
{
    [Table("PolicyParameterConditions", Schema = "pcm")]
    public class PolicyParameterCondition
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("سیاست خرید")]
        public int PolicyId { get; set; }

        [ForeignKey(nameof(PolicyId))]
        public Policy Policy { get; set; }

        [DisplayName("پارامتر")]
        public int ParameterId { get; set; }

        [ForeignKey(nameof(ParameterId))]
        public PolicyParameter Parameter { get; set; }

        [Precision(10, 3)]
        [DisplayName("مبلغ/درصد")]
        public decimal Value { get; set; }
    }
}
