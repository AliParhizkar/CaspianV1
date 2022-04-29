using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.Engine
{
    /// <summary>
    /// فیلد و نحوه ی نمایش آنها در اکتیویتی
    /// </summary>
    [Table("ActivityFields", Schema = "cmn")]
    public class ActivityField
    {
        [Key]
        public int Id { get; set; }

        public ShowType ShowType { get; set; }

        public string FieldName { get; set; }

        public int ActivityId { get; set; }

        [ForeignKey(nameof(ActivityId))]
        public virtual Activity Activity { get; set; }
    }
}
