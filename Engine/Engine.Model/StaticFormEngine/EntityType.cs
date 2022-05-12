using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.Engine
{
    /// <summary>
    /// for each entity type can declare a field of that type
    /// if IsCollection is true must declare a colection of entity
    /// </summary>
    [Table("FormsEntityFields", Schema = "cmn")]
    public class EntityField
    {
        [Key]
        public int Id { get; set; }

        public int WorkflowFormId { get; set; }

        [ForeignKey(nameof(WorkflowFormId))]
        public virtual WorkflowForm StaticForm { get; set; }

        /// <summary>
        /// Entity type fulle name that bind in forms control 
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// The name of the field declare in the form
        /// This name must as variable name in C#
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// declare entity must bind to array 
        /// </summary>
        public bool IsCollection { get; set; }
    }
}
