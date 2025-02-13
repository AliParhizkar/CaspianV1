using Caspian.Common;
using Caspian.Engine.Model;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Demo.Model
{
    [Table("Countries ", Schema = "demo")]
    public class Country: BaseEntity
    {
        [DisplayName("Title")]
        public string Title { get; set; }

        [DisplayName("Status")]
        public ActiveType ActiveType { get; set; }

        [CheckOnDelete("The country has Cities and can not removed")]
        public IList<City> Cities { get; set; }

        [CheckOnDelete("The Country has Provinces and can not be removed")]
        public IList<Province> Provinces { get; set; }

        [InverseProperty("BirthCountry")]
        [CheckOnDelete("کشور بعنوان محل تولد کارمند ثبت شده و امکان حذف آن وجود ندارد")]
        public IList<IdentificationDetail> IdentificationDetailsBirthCountry { get; set; }

        [InverseProperty("RegCountry")]
        [CheckOnDelete("کشور بعنوان محل صدور شناسنامه کارمند ثبت شده و امکان حذف آن وجود ندارد")]
        public IList<IdentificationDetail> IdentificationDetailsRegCountry { get; set; }
    }
}
