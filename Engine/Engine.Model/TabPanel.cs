using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.Engine.Model
{
    [Table("TabPanels", Schema = "cmn")]
    public class TabPanel
    {
        [Key]
        public int Id { get; set; }

        public string Title { get; set; }

        public int? ReportId { get; set; }

        [ForeignKey("ReportId")]
        public Report Report { get; set; }

        //public IList<ReportControlModel> Controls { get; set; } 
    }
}
