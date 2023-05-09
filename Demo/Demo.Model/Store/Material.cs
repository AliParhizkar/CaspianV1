using Caspian.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Demo.Model
{
    [Table("Materials", Schema = "demo")]
    public class Material
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
        /// مشخصات واحد اصلی
        /// </summary>
        [ForeignKey(nameof(MainUnitId))]
        public virtual MainUnit MainUnit { get; set; }

        /// <summary>
        /// کد واحد فرعی
        /// </summary>
        [DisplayName("واحد فرعی")]
        public int? SubunitId { get; set; }

        /// <summary>
        /// واحد فرعی
        /// </summary>
        [ForeignKey(nameof(SubunitId))]
        public virtual Subunit Subunit { get; set; }

        [DisplayName("Status")]
        public ActiveType ActiveType { get; set; }

        [CheckOnDelete("The material has receipt and can't be removed")]
        public virtual IList<ReceiptDetail> ReceiptDetails { get; set; }
    }
}
