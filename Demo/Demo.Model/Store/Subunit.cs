using Caspian.Common;
using System.ComponentModel;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Demo.Model
{
    [Table("*Subunits")]
    public class Subunit
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("عنوان")]
        public string Title { get; set; }

        /// <summary>
        /// کد واحد اصلی
        /// </summary>
        [DisplayName("واحد اصلی")]
        public int MainUnitId { get; set; }

        /// <summary>
        /// ضریب
        /// </summary>
        [DisplayName("ضریب")]
        public int Factor { get; set; }

        /// <summary>
        /// مشخصات واحد اصلی
        /// </summary>
        [ForeignKey(nameof(MainUnitId))]
        public virtual MainUnit MainUnit { get; set; }

        [DisplayName("وضعیت")]
        public ActiveType ActiveType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [CheckOnDelete("محصولاتی از نوع این واحد می باشند و امکان حذف آنها وجود ندارد.")]
        public virtual IList<Material> Materials { get; set; }
    }
}
