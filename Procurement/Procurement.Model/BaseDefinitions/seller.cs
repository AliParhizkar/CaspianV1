using Warehouse.Model;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace Procurement.Model
{
    [Table("Sellers", Schema = "pcm")]
    public class Seller
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("نوع")]
        public PersonType PersonType { get; set; }

        [DisplayName("نام")]
        public string Name { get; set; }

        [DisplayName("کد اقتصادی")]
        public string EconomicCode { get; set; }

        [DisplayName("تابعیت")]
        public Citizenship Citizenship { get; set; }

        [DisplayName("کد ملی")]
        public string IdCard { get; set; }

        [DisplayName("استان")]
        public int? ProvinceId { get; set; }

        [ForeignKey(nameof(ProvinceId))]
        public Location Province { get; set; }

        [DisplayName("شهر")]
        public int? CityId { get; set; }

        [ForeignKey(nameof(CityId))]
        public Location City { get; set; }

        [MaxLength(200), DisplayName("آدرس")]
        public string Address { get; set; }

        [DisplayName("کد پستی")]
        public string PostCode { get; set; }

        [DisplayName("پیش کد شهر")]
        public string AreaCode { get; set; }

        [DisplayName("شماره تلفن")]
        public string Tell { get; set; }

        [DisplayName("شماره تلفن2")]
        public string Tell2 { get; set; }

        [DisplayName("دورنگار")]
        public string Fax { get; set; }

        [DisplayName("پست الکترونیک")]
        public string Email { get; set; }

        [DisplayName("فعال")]
        public bool IsActive { get; set; }

        [DisplayName("توضیحات"), MaxLength(200)]
        public string Description { get; set; }
    }
}
