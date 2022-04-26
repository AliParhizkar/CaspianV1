using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.Engine
{
    /// <summary>
    /// تمامی عملیات سیستمی که کار خاصی را در سیستم انجام می دهند.
    /// </summary>
    [Table("Actors", Schema = "Common")]
    public class Actor
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// نوع عامل
        /// </summary>
        [DisplayName("نوع عامل")]
        public ActorType? ActorType { get; set; }

        /// <summary>
        /// عنوان
        /// </summary>
        [DisplayName("عنوان")]
        public string Title { get; set; }
    }
}
