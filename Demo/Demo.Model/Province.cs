using Caspian.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Demo.Model
{
    [Table("Provinces", Schema = "demo")]
    public class Province
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("Title")]
        public string Title { get; set; }

        [DisplayName("Status")]
        public ActiveType ActiveType { get; set; }

        [DisplayName("کشور")]
        public int CountryId { get; set; }

        [ForeignKey(nameof(CountryId))]
        public Country Country { get; set; }

        [CheckOnDelete("The country has Cities and can not removed")]
        public IList<City> Cities { get; set; }

        [InverseProperty("BirthProvince")]
        [CheckOnDelete("استان محل تولد کارمند می باشد و امکان حذف آن وجود ندارد")]
        public IList<IdentificationDetail> IdentificationDetailsBirthProvince { get; set; }

        [InverseProperty("RegProvince")]
        [CheckOnDelete("استان محل صدور شناسنامه کارکند می باشد و امکان حذف آن وجود ندارد")]
        public IList<IdentificationDetail> IdentificationDetailsRegProvince { get; set; }
    }
}
