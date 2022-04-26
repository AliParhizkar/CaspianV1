using Caspian.Common.Service;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.Engine
{
    [Table("TabPanels", Schema = "cmn")]
    public class TabPanel
    {
        [Key]
        public int Id { get; set; }

        public string Title { get; set; }

        public int? ReportId { get; set; }

        [ForeignKey("ReportId")]
        public virtual Report Report { get; set; }

        //public virtual IList<ReportControlModel> Controls { get; set; } 
    }
}
