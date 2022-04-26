using Caspian.Common;
using System.ComponentModel;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Demo.Model
{
    [Table("*MainUnits")]
    public class MainUnit
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("عنوان")]
        public string Title { get; set; }

        [DisplayName("وضعیت")]
        public ActiveType ActiveType { get; set; }

        /// <summary>
        /// لیست واحدهای فرعی این واحد
        /// </summary>
        [CheckOnDelete("واحد اصلی دارای واحد فرعی می باشد و امکان حذف آن وجود ندارد.")]
        public virtual IList<Subunit> Subunits { get; set; }

        /// <summary>
        /// محصولاتی که که دارای این واحد شمارش می باشند
        /// </summary>
        [CheckOnDelete("محصولی با این واحد اصلی ثبت شده و امکان حذف آن وجود ندارد")]
        public virtual IList<Material> Materials { get; set; }
    }
}
