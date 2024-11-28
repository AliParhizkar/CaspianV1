namespace Caspian.Engine
{
    public class ReportControlModel
    {
        private Type type;

        public ReportControlModel()
        {
            Dependencies = new List<ReportControlDependency>();
        }

        public ReportControlModel(Type type)
        {
            this.type = type;
            Dependencies = new List<ReportControlDependency>();
        }

        public int Id { get; set; }

        /// <summary>
        /// ***
        /// عنوان لاتین فیلد
        /// </summary>
        public string EnTitle { get; set; }

        public IList<ReportControlDependency> Dependencies { get; set; }
    }
}
