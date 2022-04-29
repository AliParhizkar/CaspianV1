using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.Engine
{
    [Table("Connectors", Schema = "cmn")]
    public class Connector
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// عنوان انشعاب شرطی
        /// </summary>
        [DisplayName("عنوان")]
        public string Title { get; set; }

        /// <summary>
        /// آیا اعتبارسنجی در انجام عملیات صورت پذیرد
        /// </summary>
        [DisplayName("اعتبارسنجی")]
        public bool CheckValidation { get; set; }

        public int ActivityId { get; set; }

        [ForeignKey(nameof(ActivityId)), InverseProperty("OutConnectors")]
        public virtual Activity Activity { get; set; }

        public ConnectorPortType PortType { get; set; }

        /// <summary>
        /// نام فیلد در حالت انشعاب شرطی
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// نوع مقایسه در حالت انشعاب شرطی
        /// </summary>
        [DisplayName("نوع مقایسه")]
        public CompareType? CompareType { get; set; }

        /// <summary>
        /// مقدار مقایسه در حالت انشعاب شرطی
        /// </summary>
        [DisplayName("ارزش")]
        public decimal? Value { get; set; }

        public int? ToActivityId { get; set; }

        [ForeignKey(nameof(ToActivityId))]
        public virtual Activity ToActivity { get; set; }

        public ConnectorPortType ToPortType { get; set; }
    }
}
