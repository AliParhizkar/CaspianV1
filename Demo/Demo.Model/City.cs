using Caspian.Common;
using Caspian.Engine.Model;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Demo.Model
{
    [Table("Cities", Schema = "demo")]
    public class City: BaseNullableEntity
    {
        [DisplayName("Title")]
        public string Title { get; set; }

        [DisplayName("Country")]
        public int CountryId { get; set; }

        [DisplayName("استان")]
        public int? ProvinceId { get; set; }

        [ForeignKey(nameof(CountryId))]
        public Country Country { get; set; }

        [ForeignKey(nameof(ProvinceId))]
        public Province Province { get; set; }

        [DisplayName("Status")]
        public ActiveType ActiveType { get; set; }

        [InverseProperty("BirthCity")]
        [CheckOnDelete("شهر محل تولد کارمند می باشد و امکان حذف آن وجود ندارد")]
        public IList<IdentificationDetail> IdentificationDetailsBirthBirthCity { get; set; }

        [InverseProperty("RegCity")]
        [CheckOnDelete("شهر محل صدور شناسنامه کارکند می باشد و امکان حذف آن وجود ندارد")]
        public IList<IdentificationDetail> IdentificationDetailsRegCity { get; set; }
    }

    //[Table("TCities")]
    //public class TCity
    //{
    //    [Key]
    //    public int Id { get; set; }

    //    //public int Number { get; set; }

    //    [MaxLength(25)]
    //    public string Name { get; set; }
    //}
}
