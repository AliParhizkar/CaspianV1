using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Demo.Model
{
    [Table("Areas", Schema = "demo")]
    public class Area
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("عنوان")]
        public string Title { get; set; }

        /// <summary>
        /// کد شهر
        /// </summary>
        [DisplayName("شهر")]
        public int CityId { get; set; }

        /// <summary>
        /// مشخصات شهر
        /// </summary>
        [ForeignKey(nameof(CityId))]
        public virtual City City { get; set; }

        [DisplayName("وضعیت")]
        public ActiveType ActiveType { get; set; }
    }
}
