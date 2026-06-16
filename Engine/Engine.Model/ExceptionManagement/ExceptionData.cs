using Caspian.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.Engine.Model
{
    [Table("ExceptionsData", Schema = "cmn")]
    public class ExceptionData
    {
        [Key]
        public int Id { get; set; }

        public SubsystemKind SubSystemKind { get; set; }

        public string SourceCodeFileName { get; set; }

        public short? LineNumber { get; set; }

        public short RepetitionTimes { get; set; }

        public string Version { get; set; }

        public string ErrorFileName { get; set; }

        public DateTime RegisterDate { get; set; }

        [CheckOnDelete(false)]
        public IList<ExceptionDetail> Details { get; set; }
    }
}
