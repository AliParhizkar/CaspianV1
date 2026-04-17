//using Caspian.Common;
//using Accounting.Model;
//using Caspian.Engine.Model;
//using System.ComponentModel;
//using System.ComponentModel.DataAnnotations.Schema;

//namespace Warehouse.Model
//{
//    [Table("GoodsRequests", Schema = "wh")]
//    public class GoodsRequest: StockFlow
//    {
//        [DisplayName("نوع سند مبناء")]
//        public ReferenceDocumentType ReferenceDocumentType { get; set; } 

//        [DisplayName("درخواست کننده")]
//        public int RequisitionerId { get; set; } ///***

//        [ForeignKey(nameof(RequisitionerId))]
//        public User Requisitioner { get; set; }

//        [DisplayName("تاریخ انبار")]
//        public DateOnly StockRoomDate { get; set; }

//        [DisplayName("بارگیری دارد")]
//        public bool Loadable { get; set; }  ///*****

//        [CheckOnDelete("این درخواست کالا دارای کالا می باشد و امکان حذف آن وجود ندارد")]
//        public ICollection<RequestGoods> Goods { get; set; }
//    }
//}
