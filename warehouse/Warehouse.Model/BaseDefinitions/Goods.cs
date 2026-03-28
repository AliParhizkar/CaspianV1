using Caspian.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Warehouse.Model
{
    [Table("Goods", Schema = "wh")]
    public class Goods
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("الگوی بارکد")]
        public int? BarcodePatternId { get; set; }

        [ForeignKey(nameof(BarcodePatternId))]
        public BarcodePattern BarcodePattern { get; set; }

        [DisplayName("کد")]
        public string Code { get; set; }

        [DisplayName("عنوان")]
        public string Name { get; set; }

        [DisplayName("واحد سنجش اصلی")]
        public int MeasurementUnitId { get; set; }

        [ForeignKey(nameof(MeasurementUnitId))]
        public MeasurementUnit MeasurementUnit { get; set; }

        [DisplayName("طبقه کالا")]
        public int ProductClassId { get; set; }

        [ForeignKey(nameof(ProductClassId))]
        public ProductClass ProductClass { get; set; }

        [DisplayName("ماهیت کالا")]
        public GoodsNature GoodsNature { get; set; }

        [DisplayName("نوع کالا")]
        public GoodsType GoodsType { get; set; }

        [DisplayName("سطح رزرو")]
        public ReservationLevel? ReservationLevel { get; set; }

        [DisplayName("رزرو براساس عامل کنترل موجودی")]
        public bool ReserveBaseOnControl { get; set; }

        [DisplayName("معلق در اسناد ورودی")]
        public bool SuspendedInIncoming { get; set; }

        [DisplayName("معلق در اسناد خروجی")]
        public bool SuspendedInOutcoming { get; set; }

        [CheckOnDelete("محصول دارای محصول جایگزین می باشد و امکان حذف آن وجود ندارد")]
        [InverseProperty(nameof(SubstituteProduct.Goods))]
        public ICollection<SubstituteProduct> Substitutes { get; set; }

        [CheckOnDelete("محصول بعنوان جایگزین تعریف شده است و امکان حذف آن وجود ندارد")]
        [InverseProperty(nameof(SubstituteProduct.SubstituteGoods))]
        public ICollection<SubstituteProduct> OtherSubstitutes { get; set; }

        [CheckOnDelete("این کالا در انبار نگهداری می شود و امکان حذف آن وجود ندارد")]
        public ICollection<GoodsPlacement> GoodsPlacements { get; set; }
    }
}
