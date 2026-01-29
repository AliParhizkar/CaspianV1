using Caspian.Common;
using Caspian.Engine;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Warehouse.Model
{
    [Table("MeasurementUnits", Schema = "wh")]
    public class MeasurementUnit
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("نام")]
        public string Name { get; set; }

        [DisplayName("بعد سنجش")]
        public MeasurementDimension MeasurementDimension { get; set; }

        [DisplayName("نام اختصار")]
        public string ShortName { get; set; }

        [DisplayName("وضعیت")]
        public ActiveStatus ActiveStatus { get; set; }

        [InverseProperty(nameof(StandardChangeUnit.MainUnit))]
        [CheckOnDelete("واحد سنجش داری تبدیل واحد استاندارد می باشد و امکان حذف آن وجود ندارد")]
        public ICollection<StandardChangeUnit> MainUnits { get; set; }

        [InverseProperty(nameof(StandardChangeUnit.OtherUnit))]
        [CheckOnDelete("واحد سنجش داری تبدیل واحد استاندارد می باشد و امکان حذف آن وجود ندارد")]
        public ICollection<StandardChangeUnit> OtherUnits { get; set; }
    }
}
