using Caspian.Common;
using Caspian.Engine;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Employment.Model
{
    /// <summary>
    /// مشخصات مدارک تحصیلی
    /// </summary>
    [Table("EducationDegrees", Schema = "emp")]
    public partial class EducationDegree
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// پایه تحصیلی
        /// </summary>
        [DisplayName("پایه تحصیلی"), ReportField]
        public BaseStudy BaseStudy { get; set; }

        /// <summary>
        /// عنوان
        /// </summary>
        [DisplayName("عنوان"), ReportField]
        public string Title { get; set; }

        /// <summary>
        /// مدارک تحصیلی
        /// </summary>
        [CheckOnDelete("مدرک تحصیلی دارای رشته می باشد و امکان حذف آن وجود ندارد")]
        public virtual IList<Major> Majors { get; set; }
    }
}
