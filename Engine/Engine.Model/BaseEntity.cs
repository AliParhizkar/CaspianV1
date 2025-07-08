using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.Engine.Model
{
    public abstract class BaseEntity
    {
        [Key]
        public int Id { get; set; }

        public DateTime UpsertDate { get; set; }

        public int UpsertUserId { get; set; }

        [ForeignKey(nameof(UpsertUserId))]
        public User UpsertUser { get; set; }
    }

    public abstract class BaseNullableEntity
    {
        [Key]
        public int Id { get; set; }

        public DateTime? UpsertDate { get; set; }

        public int? UpsertUserId { get; set; }

        [ForeignKey(nameof(UpsertUserId))]
        public User UpsertUser { get; set; }
    }
}
