using Caspian.Engine.Model;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Demo.Model
{
    [Table("Meetings", Schema = "demo")]
    public class Meeting
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public int OrderNo { get; set; }

        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }

        public bool RecordingVideoIs {  get; set; }

        public bool RecordingAudioIs { get; set; }

        public bool ActiveIs {  get; set; }

        public LearningScope LearningScope { get; set; }

        public ServerType ServerType { get; set; }

    }
}
