using Caspian.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Employment.Model
{
    /// <summary>
    /// رشته شغلی
    /// </summary>
    [Table("CareerFields", Schema = "emp")]
    public class CareerField
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("رسته شغلی")]
        public int OccupationId { get; set; }

        [ForeignKey(nameof(OccupationId))]
        public virtual Occupation Occupation { get; set; }

        [DisplayName("عنوان")]
        public string Title { get; set; }

        /// <summary>
        /// پست های سازمانی رشته شغلی
        /// </summary>
        [CheckOnDelete("رشته شغلی دارای پست سازمانی می باشد و امکان حذف آن وجود ندارد")]
        public ICollection<OrganPost> OrganPosts { get; set; }
    }
}
