using Caspian.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Procurement.Model
{
    [Table("PolicyParameters", Schema = "pcm")]
    public class PolicyParameter
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("عنوان")]
        public string Title { get; set; }

        [DisplayName("ویژگی")]
        public PolicyParameterProperty Property { get; set; }

        [CheckOnDelete("این پارامتر بعنوان شرط در سیاست خرید استفاده شده و امکان حذف آن وجود ندارد")]
        public ICollection<PolicyParameterCondition> Conditions { get; set; }
    }
}
