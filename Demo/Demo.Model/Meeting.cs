using Caspian.Engine.Model;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Demo.Model
{
    [Table("Meetings", Schema = "demo")]
    public class Meeting
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("Training Name")]
        public string Name { get; set; }

        public int OrderNo { get; set; }

        [DisplayName("Created By")]
        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; }

        [DisplayName("Recording Video")]
        public bool RecordingVideoIs {  get; set; }

        [DisplayName("Recording Audio")]
        public bool RecordingAudioIs { get; set; }

        [DisplayName("Is Active")]
        public bool ActiveIs {  get; set; }

        [DisplayName("Training Scope")]
        public LearningScope LearningScope { get; set; }

        [DisplayName("Server Type")]
        public ServerType ServerType { get; set; }

    }
}
