using Caspian.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Investment.Model
{
    [Table("Locations", Schema = "ivm")]
    public class Location
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("موقعیت جغرافیایی")]
        public int? ParentId { get; set; }

        [DisplayName("عنوان")]
        public string Title { get; set; }

        [DisplayName("کد")]
        public string Code { get; set; }

        [DisplayName("نوع موقعیت مکانی")]
        public LocationType LocationType { get; set; }

        [ForeignKey(nameof(ParentId))]
        public Location ParentLocation { get; set; }

        [CheckOnDelete("موقعیت جغرافیایی دارای زیرمجموعه می باشد و امکان حذف آن وجود ندارد")]
        public ICollection<Location> Locations { get; set; }
    }
}
