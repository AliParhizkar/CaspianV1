using Caspian.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.Engine
{
    /// <summary>
    /// تمامی عملیات سیستمی که کار خاصی را در سیستم انجام می دهند.
    /// </summary>
    [Table("Actions", Schema = "cmn")]
    public class Action
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        /// <summary>
        /// زیرسیستم
        /// </summary>
        public SubSystemKind SubSystemKind { get; set; }

        /// <summary>
        /// عنوان namespce متد
        /// </summary>
        public string Namespace { get; set; }

        /// <summary>
        /// عنوان کلاس متد
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// نام متد
        /// </summary>
        public string MethodName { get; set; }

        /// <summary>
        /// نوع عملیات دارای چکینگ یا ثبت و ...
        /// </summary>
        public SystemActionType? SystemActionType { get; set; }

        /// <summary>
        /// عنوان فارسی اکشن
        /// </summary>
        [NotMapped]
        public string FaTitle { get; set; }

        [NotMapped]
        public bool Selected { get; set; }

        public virtual Activity Activity { get; set; }
    }
}
