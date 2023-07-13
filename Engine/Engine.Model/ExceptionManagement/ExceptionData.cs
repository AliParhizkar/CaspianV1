using Caspian.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.Engine.Model
{
    [Table("ExceptionsData", Schema = "cmn")]
    public class ExceptionData
    {
        [Key]
        public int Id { get; set; }

        public SubSystemKind SubSystemKind { get; set; }

        public string FileName { get; set; }

        public string ClassName { get; set; }

        public string MethodName { get; set; }

        public string LineNumber { get; set; }

        public short RepetitionTimes { get; set; }

        public string Version { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime RegisterDate { get; set; }
    }
}
