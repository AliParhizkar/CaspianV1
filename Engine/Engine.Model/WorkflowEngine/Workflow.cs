using Caspian.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.Engine
{
    [Table("Workflows", Schema = "cmn")]
    public class Workflow
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// عنوان گردش کار
        /// </summary>
        [DisplayName("عنوان"), Unique("گردشی با این عنوان در سیستم وجود دارد."), Required]
        public string Title { get; set; }

        /// <summary>
        /// توضیحات
        /// </summary>
        [DisplayName("توضیحات")]
        public string? Descript { get; set; }

        [InverseProperty(nameof(Workflow)), CheckOnDelete("گردش دارای وضعیت می باشد و امکان حذف آن وجود ندارد.")]
        public virtual IList<Activity> Activities { get; set; }
    }
}
