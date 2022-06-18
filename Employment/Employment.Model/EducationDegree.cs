using Caspian.Engine;
using System.ComponentModel.DataAnnotations;

namespace Employment.Model
{
    public class EducationDegree
    {
        [Key]
        public int Id { get; set; }

        public BaseStudy BaseStudy { get; set; }

        public string Title { get; set; }

        public virtual IList<MarriageProperties> MarriageProperties { get; set; }

    }
}
