using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Investment.Model
{
    public class OrganPost
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("عنوان")]
        public string Title { get; set; }

        [DisplayName("کد")]
        public string Code { get; set; }

        [DisplayName("وضعیت")]
        public ActiveStatus ActiveStatus { get; set; }

        [DisplayName("شرح"), MaxLength(200)]
        public string Description { get; set; }

        public int OrganUnitId { get; set; }

        [ForeignKey(nameof(OrganUnitId))]
        public OrganUnit OrganUnit { get; set; }
    }
}
