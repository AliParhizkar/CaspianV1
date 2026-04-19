using Caspian.Common;
using Warehouse.Model;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Procurement.Model
{
    [Table("Services", Schema = "pcm")]
    public class Service
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("کد")]
        public string Code { get; set; }

        [DisplayName("نام")]
        public string Name { get; set; }

        [DisplayName("واحد سنجش اصلی")]
        public int MeasurementUnitId { get; set; }

        [ForeignKey(nameof(MeasurementUnitId))]
        public MeasurementUnit MeasurementUnit { get; set; }

        [CheckOnDelete("این خدمت بعنوان اقلام خریدنی ثبت شده است و امکان حذف آن وجود ندارد")]
        public ICollection<ProcurementItem> ProcurementItems { get; set; }
    }
}
