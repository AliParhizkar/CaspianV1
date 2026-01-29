using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Warehouse.Model
{
    [Table("StandardChangeUnits", Schema = "wh")]
    public class StandardChangeUnit
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("نام")]
        public string Name { get; set; }

        [DisplayName("واحد سنجش اصلی")]
        public int MainUnitId { get; set; }

        [ForeignKey(nameof(MainUnitId))]
        public MeasurementUnit MainUnit { get; set; }

        [DisplayName("واحد سنجش")]
        public int OtherUnitId { get; set; }

        [DisplayName("نسبت")]
        public decimal Rate { get; set; }

        [ForeignKey(nameof(OtherUnitId))]
        public MeasurementUnit OtherUnit { get; set; }
    }
}
