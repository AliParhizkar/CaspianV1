using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.Engine.Model
{
    [Table("ReportGroupsParameters", Schema = "cmn")]
    public class ReportGroupParameter
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("Report group")]
        public int ReportGroupId { get; set; }

        [ForeignKey(nameof(ReportGroupId))]
        public virtual ReportGroup ReportGroup { get; set; }

        /// <summary>
        /// عنوان لاتین فیلد
        /// </summary>
        [DisplayName("عنوان لاتین")]
        public string TitleEn { get; set; }

        [DisplayName("Alias")]
        public string Alias { get; set; }

        /// <summary>
        /// This Field is used for multi level report 
        /// </summary>
        [DisplayName("Is key")]
        public bool IsKey { get; set; }
    }
}
