using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Warehouse.Model
{
    [Table("Sellers", Schema = "wh")]
    public class Seller
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("نوع")]
        public SellerType SellerType { get; set; }

        [DisplayName("نام شرکت")]
        public string Name { get; set; }

        [DisplayName("کد اقتصادی")]
        public string EconomicCode { get; set; }

        [DisplayName("شماره ثبت")]
        public string RegistrationNo { get; set; }

        [DisplayName("تابعیت")]
        public Citizenship? Citizenship { get; set; }

        [DisplayName("شناسه/کدملی")]
        public string IdCard { get; set; }

        [DisplayName("استان")]
        public int? ProvinceId { get; set; }

        [ForeignKey(nameof(ProvinceId))]
        public Province Province { get; set; }

        [DisplayName("شهر")]
        public int? CityId { get; set; }

        [ForeignKey(nameof(CityId))]
        public City City { get; set; }

        [DisplayName("آدرس")]
        public string Address { get; set; }

        [DisplayName("کد پستی")]
        public string Zipcode { get; set; }

        [DisplayName("پیش کد شهر")]
        public string PreCode { get; set; }

        [DisplayName("تلفن 1")]
        public string Tel1 { get; set; }

        [DisplayName("تلفن 2")]
        public string Tel2 { get; set; }
    }
}
