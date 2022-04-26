using Caspian.Common;
using System.ComponentModel;
using Caspian.Common.Security;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.Engine
{
    /// <summary>
    /// ردیابی مراحل گردش کار
    /// </summary>
    [Table("WorkflowTraces", Schema = "cmn")]
    public class WorkflowTrace
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        /// <summary>
        /// شماره ی ردیابی
        /// </summary>
        [DisplayName("شماره ی ردیابی")]
        public int TraceId { get; set; }

        /// <summary>
        /// کد فرستنده
        /// </summary>
        [CheckOnInsert("فرستنده ای با کد {0} در سیستم وجود ندارد")]
        public int? SenderId { get; set; }

        /// <summary>
        /// مشخصات فرستنده
        /// </summary>
        [ForeignKey(nameof(SenderId))]
        public virtual CustomUser Sender { get; set; }

        /// <summary>
        /// کد گیرنده
        /// </summary>
        [CheckOnInsert("گیرنده ای با کد {0} در سیستم تعریف نشده است")]
        public int? ReciverId { get; set; }

        /// <summary>
        /// مشخصات گیرنده
        /// </summary>
        [ForeignKey(nameof(ReciverId))]
        public virtual CustomUser Reciver { get; set; }

        [CheckOnInsert("کانکشنی با کد {0} در سیستم تعریف نشده است")]
        public int ConnectorId { get; set; }

        [ForeignKey(nameof(ConnectorId))]
        public virtual Connector Connector { get; set; }

        public string FileName { get; set; }

        /// <summary>
        /// تاریخ ارجاع
        /// </summary>
        [DisplayName("تاریخ ارجاع")]
        public DateTime RegisterDate { get; set; }
    }
}
