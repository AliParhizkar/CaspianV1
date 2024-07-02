using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.Engine.Model
{
    [Table("Fonts", Schema = "cmn")]
    public class CaspianFont
    {
        [Key]
        public short Id { get; set; }

        public string Name { get; set; }
    }
}
