using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Employment.Model
{
    /// <summary>
    /// مشخصات کارمندان
    /// </summary>
    [Table("Employees", Schema = "emp")]
    public class Employee
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// نام
        /// </summary>
        [DisplayName("نام")]
        public string FName { get; set; }

        /// <summary>
        /// نام خانوادگی
        /// </summary>
        [DisplayName("نام خانوادگی")]
        public string LName { get; set; }

        /// <summary>
        /// جنسیت
        /// </summary>
        [DisplayName("جنسیت")]
        public Gender Gender { get; set; }

        /// <summary>
        /// تصویر
        /// </summary>
        [DisplayName("تصویر")]
        public byte[] Image { get; set; }
    }
}
