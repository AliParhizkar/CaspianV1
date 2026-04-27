using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Procurement.Model
{
    public class PolicyParameter
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("عنوان")]
        public string Title { get; set; }

        [DisplayName("ویژگی")]
        public PolicyParameterProperty Property { get; set; }


    }
}
