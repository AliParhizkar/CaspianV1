using Caspian.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.Engine
{
    [Table("LookupTypes", Schema = "cmn")]
    public class LookupType
    {
        [Key]
        public int Id { get; set; }

        public string Title { get; set; }

        public string EntityTypeName { get; set; }

        public string LookupTypeName { get; set; }

        [CheckOnDelete("این نوع دارای کنترل می باشد و امکان حذف آن وجود ندارد.")]
        public virtual IList<BlazorControl> BlazorControls { get; set;}
    }
}
