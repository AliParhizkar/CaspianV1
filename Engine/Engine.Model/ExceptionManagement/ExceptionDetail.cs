using Caspian.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.Engine.Model
{
    [Table("ExceptionDetails", Schema = "cmn")]
    public class ExceptionDetail
    {
        [Key]
        public int Id { get; set; }

        public int ExceptionDataId { get; set; }

        [ForeignKey(nameof(ExceptionDataId))]
        public virtual ExceptionData ExceptionData { get; set; }



        public virtual User User { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime RegisterDate { get; set; }
    }
}
