using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.Engine
{
    /// <summary>
    /// for each entity type can declare a field of that type
    /// if IsCollection is true must declare a colection of entity
    /// </summary>
    [Table("WfFormsEntityFields", Schema = "cmn")]
    public class WfFormEntityField
    {
        [Key]
        public int Id { get; set; }

        public int WorkflowFormId { get; set; }

        [ForeignKey(nameof(WorkflowFormId))]
        public virtual WorkflowForm WorkflowForm { get; set; }

        /// <summary>
        /// Entity type fulle name that bind in forms control 
        /// </summary>
        [DisplayName("نوع موجودیت")]
        public string EntityFullName { get; set; }

        /// <summary>
        /// Name of entity that show on form generator 
        /// </summary>
        [DisplayName("عنوان")]
        public string Title { get; set; }

        /// <summary>
        /// The name of the field declare in the form
        /// This name must as variable name in C#
        /// </summary>
        [DisplayName("عنوان فیلد")]
        public string FieldName { get; set; }

        /// <summary>
        /// declare entity must bind to array 
        /// </summary>
        public bool IsCollection { get; set; }
    }
}
