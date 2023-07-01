using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Employment.Model
{
    [Table("BaseNumbers", Schema = "emp")]
    public class BaseNumber
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// رتبه
        /// </summary>
        [DisplayName("رتبه")]
        public BaseType BaseType { get; set; }

        /// <summary>
        /// پایه تحصیلی
        /// </summary>
        [DisplayName("پایه تحصیلی")]
        public BaseStudy BaseStudy { get; set; }

        /// <summary>
        /// عدد مبناء
        /// </summary>
        [DisplayName("عدد مبنا")]
        public int Factor { get; set; }
    }
}
