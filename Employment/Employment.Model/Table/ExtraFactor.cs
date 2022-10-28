using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Employment.Model
{
    /// <summary>
    /// ضرایب فوق العاده
    /// </summary>
    [Table("ExtraFactors", Schema = "emp")]
    public class ExtraFactor
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// رتبه
        /// </summary>
        [DisplayName("رتبه")]
        public BaseType BaseType { get; set; }

        /// <summary>
        /// فوق العاده شغل
        /// </summary>
        [DisplayName("فوق العاده شغل")]
        [Column("Jobfactor", TypeName = "numeric(5, 3)")]
        public decimal Jobfactor { get; set; }

        /// <summary>
        /// فوق العاده جذب
        /// </summary>
        [DisplayName("فوق العاده جذب")]
        [Column("SwallowFactor", TypeName = "numeric(5, 3)")]
        public decimal SwallowFactor{get;set;}

        /// <summary>
        /// فوق العاده ویژه
        /// </summary>
        [DisplayName("فوق العاده ویژه")]
        [Column("SpesialFactor", TypeName = "numeric(5, 3)")]
        public decimal SpesialFactor { get;set;}
    }
}
