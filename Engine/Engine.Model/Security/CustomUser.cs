using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.Common.Security
{
    /// <summary>
    /// مشخصات عضویت کاران سیستم
    /// </summary>
    [Table("CustomUsers", Schema = "cmn")]
    public class CustomUser
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        /// <summary>
        /// نام
        /// </summary>
        [DisplayName("نام"), Required]
        public string FName { get; set; }

        /// <summary>
        /// نام خانوادگی
        /// </summary>
        [DisplayName("نام خانوادگی"), Required]
        public string LName { get; set; }

        /// <summary>
        /// نام و نام خانوادگی
        /// </summary>
        [NotMapped, DisplayName("نام نام خانوادگی")]
        public string FLName { get; set; }

        /// <summary>
        /// نام کاربری
        /// </summary>
        [DisplayName("نام کاربری"), Required, Unique("کاربری با این نام کاربری ثبت نام کرده است.")]
        public string UserName { get; set; }

        /// <summary>
        /// نام کاربری
        /// </summary>
        [DisplayName("تلفن همراه"), Required, Unique("کاربری با این شماره تلفن همراه ثبت نام شده است.")]
        public string MobileNumber { get; set; }

        /// <summary>
        /// کلمه عبور
        /// </summary>
        [DisplayName("کلمه عبور")]
        public string PassWord { get; set; }

        /// <summary>
        /// کد ملی
        /// </summary>
        [DisplayName("کد ملی")]
        public string NationalCode { get; set; }

        /// <summary>
        /// تصویر
        /// </summary>
        [DisplayName("تصویر")]
        public byte[] Image { get; set; }
    }
}