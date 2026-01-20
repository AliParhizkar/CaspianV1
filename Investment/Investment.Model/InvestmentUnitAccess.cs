using Caspian.Engine.Model;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Investment.Model
{
    [Table("InvestmentUnitAccess", Schema = "ivm")]
    public class InvestmentUnitAccess
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; }

        public int InvestmentUnitId { get; set; }

        [ForeignKey(nameof(InvestmentUnitId))]
        public InvestmentUnit InvestmentUnit { get; set; }
    }
}
