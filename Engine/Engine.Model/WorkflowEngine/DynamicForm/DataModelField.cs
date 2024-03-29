﻿using Caspian.Common;
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

        public int? DataModelId { get; set; }

        [ForeignKey(nameof(DataModelId))]
        public virtual DataModel DataModel { get; set; }

        /// <summary>
        /// Entity type fulle name that bind in forms control 
        /// </summary>
        [DisplayName("Entity name")]
        public string EntityFullName { get; set; }

        [DisplayName("Field type")]
        public DataModelFieldType? FieldType { get; set; }

        /// <summary>
        /// Name of entity that show on form generator 
        /// </summary>
        [DisplayName("Title")]
        public string Title { get; set; }

        /// <summary>
        /// The name of the field declare in the form
        /// This name must as variable name in C#
        /// </summary>
        [DisplayName("Field name")]
        public string FieldName { get; set; }

        [DisplayName("Entity type")]
        public int? EntityTypeId { get; set; }

        [ForeignKey(nameof(EntityTypeId))]
        public virtual EntityType EntityType { get; set; }

        /// <summary>
        /// Check Entity type can be as Details
        /// </summary>
        public bool IsDetails { get; set; }

        [CheckOnDelete("The field set bind to control and can not be removed")]
        public virtual IList<BlazorControl> BlazorControls { get; set; }

        [CheckOnDelete("The field has option(s) and can not be removed")]
        public virtual IList<DataModelOption> DataModelOptions { get; set; } 
    }
}
