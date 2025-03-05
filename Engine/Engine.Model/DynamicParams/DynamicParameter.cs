using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.Engine
{
    /// <summary>
    /// dynamic parameters that is created by user
    /// </summary>
    [Table("DynamicParameters", Schema = "cmn")]
    public class DynamicParameter
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("عنوان")]
        public string Title { get; set; }
    }
}
