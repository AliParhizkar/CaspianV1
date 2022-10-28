using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Employment.Model
{
    [Table("BaseStudyFactors", Schema = "emp")]
    public class BaseStudyFactor
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// پایه تحصیلی
        /// </summary>
        [DisplayName("پایه تحصیلی")]
        public BaseStudy BaseStudy { get; set; }

        /// <summary>
        /// ضریب
        /// </summary>
        [DisplayName("ضریب")]
        public int Factor { get; set; }
    }
}
