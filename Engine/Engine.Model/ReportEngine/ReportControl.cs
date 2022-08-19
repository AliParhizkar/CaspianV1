using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.Engine.Model
{
    [Table("ReportControls", Schema = "cmn")]
    public class ReportControl
    {
        [Key]
        public int Id { get; set; }

        public int? Left { get; set; }

        public int? Top { get; set; }

        public int? Width { get; set; }

        public int? Height { get; set; }
        
        public string FaTitle { get; set; }

        public string EnTitle { get; set; }

        public int TabPanelId { get; set; }

        [ForeignKey("TabPanelId")]
        public virtual TabPanel TabPanel { get; set; }
    }
}
