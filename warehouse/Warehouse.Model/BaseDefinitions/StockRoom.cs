using Caspian.Common;
using Caspian.Engine.Model;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Warehouse.Model
{
    [Table("StockRooms", Schema = "wh")]
    public class StockRoom
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("کد انبار")]
        public string Code { get; set; }

        [DisplayName("نام انبار")]
        public string Title { get; set; }

        [DisplayName("مرکز نگهداری کالا")]
        public int KeepCenterId { get; set; }

        [ForeignKey(nameof(KeepCenterId))]
        public KeepCenter KeepCenter { get; set; }

        [DisplayName("سرپرست انبار")]
        public int? ManagerId { get; set; }

        [ForeignKey(nameof(ManagerId))]
        public User Manager { get; set; }

        [DisplayName("شعبه")]
        public int BranchId { get; set; }

        [ForeignKey(nameof(BranchId))]
        public Branch Branch { get; set; }

        [DisplayName("امکان عضویت در بخش های میانی")]
        public bool MemberOfMiddlePart { get; set; }

        [DisplayName("امکان عضویت در بیش از یک بخش")]
        public bool MemberInManyParts { get; set; }

        [CheckOnDelete("انبار دارای محل فیزیکی کالا می باشد و امکان حذف آن وجود ندارد")]
        public ICollection<MaterialLocation> MaterialAddresses{ get; set; }
    }
}
