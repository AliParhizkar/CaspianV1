using Caspian.Engine;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Employment.Model
{
    /// <summary>
    /// رشته تحصیلی
    /// </summary>
    [Table("Majors", Schema = "emp")]
    public class Major
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("مدرک تحصیلی")]
        public int EducationDegreeId { get; set; }

        [DisplayName("عنوان"), ReportField]
        public string Title { get; set; }

        [ForeignKey(nameof(EducationDegreeId)), ReportField("مدرک تحصیلی")]
        public virtual EducationDegree EducationDegree { get; set; }
    }
}
