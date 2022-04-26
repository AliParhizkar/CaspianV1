using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.Engine
{
    /// <summary>
    /// تمامی عملیات سیستمی که کار خاصی را در سیستم انجام می دهند.
    /// </summary>
    [Table("TaskOperations", Schema = "cmn")]
    public class TaskOperation
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// کد کاربر
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// کد حالت عملیات
        /// </summary>
        public int ActivityId { get; set; }

        /// <summary>
        /// مشخصات حالت عملیات
        /// </summary>
        [ForeignKey(nameof(ActivityId))]
        public virtual Activity Activity { get; set; }

        /// <summary>
        /// کد عملیات انجام شده
        /// </summary>
        public int ConnectorId { get; set; }

        /// <summary>
        /// مشخصات عملیات انجام شده
        /// </summary>
        [ForeignKey(nameof(ConnectorId))]
        public virtual Connector Connector { get; set; }

        public int No { get; set; }

        public bool IsLaste { get; set; }
    }
}
