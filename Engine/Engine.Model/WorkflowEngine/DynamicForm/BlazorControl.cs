using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.Engine
{
    [Table("BlazorControls", Schema = "cmn")]
    public class BlazorControl
    {
        [Key]
        public int Id { get; set; }

        public string Caption { get; set; }
        
        public string? Description { get; set; }

        public string? CustomeFieldName { get; set; }
        
        public ControlType ControlType { get; set; }

        public int DataModelFieldId { get; set; }

        /// <summary>
        /// Field that declare in form
        /// control can bind to properties of this field 
        /// </summary>
        [ForeignKey(nameof(DataModelFieldId))]
        public virtual DataModelField DataModelField { get; set; }

        /// <summary>
        /// The property name of entity that control bind to it
        /// </summary>
        public string PropertyName { get; set; }

        public int? DynamicParameterId { get; set; }

        [ForeignKey(nameof(DynamicParameterId))]
        public virtual DynamicParameter DynamicParameter { get; set; }

        public string TextExpression { get; set; }

        public string FilterExpression { get; set; }

        public bool MultiLine { get; set; }

        public byte? Height { get; set; }

        public string OnChange { get; set; }

        public int? HtmlColumnId { get; set; }

        [ForeignKey(nameof(HtmlColumnId))]
        public virtual HtmlColumn HtmlColumn { get; set; }
    }
}
