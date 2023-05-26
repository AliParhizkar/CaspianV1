using Caspian.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.Engine
{
    /// <summary>
    /// for each entity type can declare a field of that type
    /// if IsCollection is true must declare a colection of entity
    /// </summary>
    [Table("DataModelFields", Schema = "cmn")]
    public class DataModelField
    {
        [Key]
        public int Id { get; set; }

        public int DataModelId { get; set; }

        [ForeignKey(nameof(DataModelId))]
        public virtual DataModel DataModel { get; set; }

        /// <summary>
        /// Entity type fulle name that bind in forms control 
        /// </summary>
        [DisplayName("نام موجودیت")]
        public string EntityFullName { get; set; }

        [DisplayName("نام فیلد")]
        public DataModelFieldType? FieldType { get; set; }

        /// <summary>
        /// Name of entity that show on form generator 
        /// </summary>
        [DisplayName("عنوان")]
        public string Title { get; set; }

        /// <summary>
        /// The name of the field declare in the form
        /// This name must as variable name in C#
        /// </summary>
        [DisplayName("نام فیلد")]
        public string FieldName { get; set; }

        /// <summary>
        /// declare entity must bind to array 
        /// </summary>
        public bool IsCollection { get; set; }

        [CheckOnDelete("فیلد به کنترل تخصیص داده شده و امکان حذف آن وجود ندارد")]
        public virtual IList<BlazorControl> BlazorControls { get; set; }

        [CheckOnDelete("فیلد داده ای دارای گزینه است و امکان حذف آن وجود ندارد")]
        public virtual IList<DataModelOption> DataModelOptions { get; set; } 
    }
}
