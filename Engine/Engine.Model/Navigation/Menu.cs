using Caspian.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.Engine.Model
{
    [Table("Menus", Schema = "cmn")]
    public class Menu
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// عنوان فارسی منو
        /// </summary>
        [DisplayName("عنوان")]
        public string Title { get; set; }

        /// <summary>
        /// آدرس صفحه
        /// </summary>
        [DisplayName("آدرس صفحه")]
        public string Source { get; set; }

        /// <summary>
        /// کد منوی اصلی
        /// </summary>
        [DisplayName("منوی اصلی")]
        public int? MenuCategoryId { get; set; }

        /// <summary>
        /// مشخصات منوی اصلی
        /// </summary>
        [ForeignKey(nameof(MenuCategoryId))]
        public virtual  MenuCategory MenuCategory { get; set; }

        public int Ordering { get; set; }

        /// <summary>
        /// نمایش در منو
        /// </summary>
        [DisplayName("نمایش در منو")]
        public bool ShowonMenu { get; set; }

        /// <summary>
        /// آدرسهایی که در دیتابیس وجود دارند ولی در سیستم نیستند
        /// </summary>
        [DisplayName("آدرس نامعتبر")]
        public bool InvalidAddress { get; set; }

        [CheckOnDelete("منو دارای دستیابی می باشد و امکان حذف آن وجود ندارد")]
        public IList<UserAccessibility> Accessibilities { get; set; }
    }
}
