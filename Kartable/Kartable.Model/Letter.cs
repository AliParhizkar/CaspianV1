using Caspian.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kartable.Model
{
    [Table("Letters", Schema = "kar")]
    public class Letter
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// عنوان
        /// </summary>
        [DisplayName("عنوان")]
        public string Title { get; set; }

        /// <summary>
        /// متن نامه
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// الصاق های نامه
        /// </summary>
        [CheckOnDelete("نامه دااری الصاق می باشد و امکان حذف آن وجود ندارد.")]
        public virtual ICollection<LetterAttachment> Attachments { get;set; }
    }
}