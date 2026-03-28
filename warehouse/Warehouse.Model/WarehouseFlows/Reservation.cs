using Accounting.Model;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Warehouse.Model
{
    [Table("Reservations", Schema = "wh")]
    public class Reservation
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("تاریخ رزور")]
        public DateOnly DateOnly { get; set; }

        [DisplayName("تاریخ اعتبار")]
        public DateOnly? ExpirationDate { get; set; }

        [DisplayName("شماره")]
        public string ReservationNo { get; set; }

        [DisplayName("نوع طرف مقابل")]
        public OtherPartyType OtherPartyType { get; set; }

        [DisplayName("طرف مقابل")]
        public int? CostCenterId { get; set; }

        [ForeignKey(nameof(CostCenterId))]
        public CostCenter CostCenter { get; set; }

        [DisplayName("مبنای رزرو")]
        public ReservationBasis ReservationBasis { get; set; }

        [DisplayName("نوع رزرو")]
        public ReservationType ReservationType { get; set; }

        [DisplayName("مرکز نگهداری")]
        public int KeepCenterId { get; set; }

        [ForeignKey(nameof(KeepCenterId))]
        public KeepCenter KeepCenter { get; set; }

        [DisplayName("انبار")]
        public int? StockRoomId { get; set; }

        [ForeignKey(nameof(StockRoomId))]
        public StockRoom StockRoom { get; set; }

        [DisplayName("شعبه")]
        public string Branch { get; set; }
    }
}
