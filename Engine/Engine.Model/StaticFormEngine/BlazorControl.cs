using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.Engine
{
    [Table("BlazorControls", Schema = "cmn")]
    public class Component
    {
        [Key]
        public int Id { get; set; }

        public string Caption { get; set; }
        
        public string? Description { get; set; }
        
        public ControlType ControlType { get; set; }

        public int? WfFormEntityFieldId { get; set; }

        /// <summary>
        /// Field that declare in form
        /// control can bind to properties of this field 
        /// </summary>
        [ForeignKey(nameof(WfFormEntityFieldId))]
        public virtual WfFormEntityField WfFormEntityField { get; set; }

        /// <summary>
        /// The property name of entity that control bind to it
        /// </summary>
        public string PropertyName { get; set; }

        public int? HtmlColumnId { get; set; }

        [ForeignKey(nameof(HtmlColumnId))]
        public virtual HtmlColumn HtmlColumn { get; set; }
    }
}
