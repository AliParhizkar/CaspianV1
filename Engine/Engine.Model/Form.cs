using Caspian.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.Engine
{
    [Table("Forms", Schema = "cmn")]
    public class Form
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("عنوان")]
        public string Title { get; set; }

        /// <summary>
        /// عنوان فایل
        /// </summary>
        [DisplayName("عنوان فایل")]
        public string? FileName { get; set; }

        /// <summary>
        /// گروه فرم
        /// </summary>
        [DisplayName("گروه فرم"), Required]
        public int FormGroupId { get; set; }

        /// <summary>
        /// مشخصات فرم
        /// </summary>
        [ForeignKey("FormGroupId")]
        public virtual FormGroup FormGroup { get; set; }
        
        [CheckOnDelete("فرم دارای فیلدهای پویا می باشد و امکان حدف آن وجود ندارد.")]
        public virtual IList<ActivityDynamicField> DynamicFields { get; set; }
    }
}
