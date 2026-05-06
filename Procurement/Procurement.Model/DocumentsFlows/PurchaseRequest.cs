using Warehouse.Model;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Caspian.Engine.Model;

namespace Procurement.Model
{
    public class PurchaseRequest  
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("شماره سفارش")]
        public string No { get; set; }

        [DisplayName("تاریخ سفارش")]
        public DateOnly Date { get; set; }

        [DisplayName("نوع قلم")]
        public ProcurementItemType ProcurementItemType { get; set; }

        [DisplayName("مرکز درخواست کننده")]
        public int KeepCenterId { get; set; }

        [ForeignKey(nameof(KeepCenterId))]
        public KeepCenter KeepCenter { get; set; }

        [DisplayName("نوع طرف مقابل")]
        public OtherPartyType OtherPartyType { get; set; }


    }
}
