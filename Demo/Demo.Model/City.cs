using Caspian.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Demo.Model
{
    [Table("Cities", Schema = "demo")]
    public class City
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("عنوان")]
        public string Title { get; set; }

        /// <summary>
        /// کد استان
        /// </summary>
        [DisplayName("استان")]
        public int ProvinceId { get; set; }

        /// <summary>
        /// مشخصات استان
        /// </summary>
        [ForeignKey(nameof(ProvinceId))]
        public virtual Province Province { get; set; }

        [DisplayName("وضعیت")]
        public ActiveType ActiveType { get; set; }

        /// <summary>
        /// لیست مناطق شهر
        /// </summary>
        [CheckOnDelete("شهر داری منطقه می باشد و امکان حذف آن وجود ندارد.")]
        public virtual IList<Area> Areas { get; set; }
    }
}
