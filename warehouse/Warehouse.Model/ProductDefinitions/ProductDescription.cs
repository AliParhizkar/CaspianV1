using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Warehouse.Model
{
    public class ProductDescription
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("نام")]
        public string Name { get; set; }

        [DisplayName("نوع")]
        public DescriptionType DescriptionType { get; set; }

        [DisplayName("مقدار")]
        public string Value { get; set; }
    }
}
