using Caspian.Common;
using Microsoft.EntityFrameworkCore;

namespace Caspian.Engine.Model
{
    public class Context: MyContext
    {
        public DbSet<Menu> Menus { get; set; }

        public DbSet<MenuCategory> MenuCategories { get; set; }

        public DbSet<Report> Reports { get; set; }

        public DbSet<ReportParam> ReportParams { get; set; }

        public DbSet<ReportGroup> ReportGroups { get; set; }

        public DbSet<Rule> Rules { get; set; }

        public DbSet<TabPanel> TabPanels { get; set; }

        public DbSet<ReportControl> ReportControls { get; set; }

        public DbSet<Token> Tokens { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<UserLogin> UsersLogins { get; set; }

        public DbSet<Role> Roles { get; set; }

        public DbSet<Activity> Activities { get; set; }

        public DbSet<NodeConnector> Connectors { get; set; }

        public DbSet<TaskOperation> TaskOperations { get; set; }

        public DbSet<Workflow> Workflows { get; set; }

        public DbSet<DynamicParameter> DynamicParameters { get; set; }

        public DbSet<MenuAccessibility> MenuAccessibilities { get; set; }

        public DbSet<DynamicParameterValue> DynamicParametersValues { get; set; }

        public DbSet<WorkflowForm> WorkflowForms { get; set; }

        public DbSet<DataModel> DataModels { get; set; }

        public DbSet<DataParameter> DataParameters { get; set; }

        public DbSet<DataParameterValue> DataParameterValues { get; set; }

        public DbSet<HtmlColumn> HtmlColumns { get; set; }

        public DbSet<ExceptionData> ExceptionDatas { get; set; }

        public DbSet<ExceptionDetail> ExceptionDetail { get; set; }
    }
}
