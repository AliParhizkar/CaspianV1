using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.Engine
{
    /// <summary>
    /// 
    /// </summary>
    [Table("ActivitiesDynamicFields", Schema = "cmn")]
    public class ActivityDynamicField
    {
        [Key]
        public int Id { get; set; }

        public int ActivityId { get; set; }

        [ForeignKey(nameof(ActivityId))]
        public virtual Activity Activity { get; set; }

        /// <summary>
        /// کد کنترل در فایل فرم کنترل
        /// </summary>
        public int ControlId { get; set; }

        public string Title { get; set; }

        /// <summary>
        /// نوع نمایش فیلدهای فرم
        /// </summary>
        public ShowType? ShowType { get; set; }

        /// <summary>
        /// زمان نمایش فیلدهای فرم
        /// </summary>
        public ShowTime? ShowTime { get; set; }
    }
}
