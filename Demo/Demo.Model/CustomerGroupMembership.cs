using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Demo.Model
{
    [Table("*CustomerGroupsMembership")]
    public class CustomerGroupMembership
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// کد مشتری
        /// </summary>
        [DisplayName("مشتری")]
        public int CustomerId { get; set; }

        /// <summary>
        /// مشخصات مشتری
        /// </summary>
        [ForeignKey(nameof(CustomerId))]
        public virtual Customer Customer { get; set; }

        /// <summary>
        /// کد گروه مشتریان
        /// </summary>
        [DisplayName("گروه مشتری")]
        public int CustomerGroupId { get; set; }

        /// <summary>
        /// مشخصات گروه مشتریان
        /// </summary>
        [ForeignKey(nameof(CustomerGroupId))]
        public virtual CustomerGroup CustomerGroup { get; set; }
    }
}
