using Caspian.Common;
using Caspian.Engine.Model;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
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

        [InverseProperty(nameof(IdentificationDetail.BirthCountry))]
        [CheckOnDelete("کشور بعنوان محل تولد کارمند ثبت شده و امکان حذف آن وجود ندارد")]
        public IList<IdentificationDetail> IdentificationDetailsBirthCountry { get; set; }

        [InverseProperty(nameof(IdentificationDetail.RegCountry))]
        [CheckOnDelete("کشور بعنوان محل صدور شناسنامه کارمند ثبت شده و امکان حذف آن وجود ندارد")]
        public IList<IdentificationDetail> IdentificationDetailsRegCountry { get; set; }
    }

    [Table("TestProducts")]
    public class TestProduct
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        [InverseProperty(nameof(SameProduct.Product))]
        public IList<SameProduct> Products { get; set; }

        [InverseProperty(nameof(SameProduct.MaterialSame))]
        public IList<SameProduct> SameProducts { get; set; }
    }

    [Table("SameProducts")]
    public class SameProduct
    {
        [Key]
        public int Id { get; set; }

        public int ProductId { get; set; }

        [ForeignKey(nameof(ProductId))]
        public TestProduct Product { get; set; }

        public int MaterialSameId { get; set; }

        [ForeignKey(nameof(MaterialSameId))]
        public TestProduct MaterialSame { get; set; }
    }
}
