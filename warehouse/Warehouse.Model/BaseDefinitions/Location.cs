using Caspian.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Warehouse.Model
{
    [Table("Locations", Schema = "wh")]
    public class Location
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("محل جغرافیایی")]
        public int? ParentId { get; set; }

        [DisplayName("عنوان")]
        public string Title { get; set; }

        [DisplayName("کد")]
        public string Code { get; set; }

        [DisplayName("نوع محل مکانی")]
        public LocationType LocationType { get; set; }

        [ForeignKey(nameof(ParentId))]
        public Location ParentLocation { get; set; }

        [CheckOnDelete("محل جغرافیایی دارای زیرمجموعه می باشد و امکان حذف آن وجود ندارد")]
        public ICollection<Location> Locations { get; set; }

        [CheckOnDelete("محل جغرافیایی دارای مرکزنگهداری می باشد و امکان حذف آن وجود ندارد")]
        public ICollection<KeepCenter> KeepCenters { get; set; }
    }
}
