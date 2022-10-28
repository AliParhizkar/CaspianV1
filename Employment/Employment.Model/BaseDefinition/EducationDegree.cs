using Caspian.Common;
using Caspian.Engine;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Employment.Model
{
    public class EducationDegree
    {
        [Key]
        public int Id { get; set; }

        public BaseStudy BaseStudy { get; set; }

        [DisplayName("عنوان")]
        public string Title { get; set; }

        [CheckOnDelete("این مدرک تحصیلی بعنوان سوابق فرزندان ثبت شده و امکان حذف آن وجود ندارد")]
        public virtual IList<MarriageProperties> MarriageProperties { get; set; }

        /// <summary>
        /// مدارک تحصیلی
        /// </summary>
        [CheckOnDelete("مدرک تحصیلی دارای رشته می باشد و امکان حذف آن وجود ندارد")]
        public virtual IList<Major> Majors { get; set; }

    }
}
