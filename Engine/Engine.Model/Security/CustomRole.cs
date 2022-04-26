using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.Common.Security
{
    /// <summary>
    /// مشخصات نقش های موجود در سیستم
    /// </summary>
    [Table("CustomRoles", Schema = "cmn")]
    public class CustomRole
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        /// <summary>
        /// عنوان نقشهای سامانه
        /// </summary>
        [DisplayName("عنوان"), Required, Unique("نقشی با این عنوان قبلا ثبت شده است.")]
        public string Title { get; set; }

        public string DefaultUrl { get; set; }

        /// <summary>
        /// شرح
        /// </summary>
        [MaxLength(150), DisplayName("شرح")]
        public string Descript { get; set; }
    }
}
