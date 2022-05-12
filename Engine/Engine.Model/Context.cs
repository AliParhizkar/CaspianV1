using Caspian.Common;
using Caspian.Common.Security;
using Caspian.Common.Navigation;
using Microsoft.EntityFrameworkCore;

namespace Caspian.Engine
{
    public class Context: MyContext
    {
        public DbSet<Menu> Menus { get; set; }

        public DbSet<MenuCategory> MenuCategories { get; set; }

        public DbSet<Report> Reports { get; set; }

        public DbSet<ReportGroup> ReportGroups { get; set; }

        public DbSet<Rule> Rules { get; set; }

        public DbSet<TabPanel> TabPanels { get; set; }

        public DbSet<ReportControl> ReportControls { get; set; }

        public DbSet<Token> Tokens { get; set; }

        public DbSet<CustomRole> CustomRoles { get; set; }

        public DbSet<CustomUser> CustomUsers { get; set; }

        public DbSet<Action> Actions { get; set; }

        public DbSet<Activity> Activities { get; set; }

        public DbSet<ActivityAccess> ActivityAccesses { get; set; }

        public DbSet<ActivityDynamicField> ActivityDynamicFields { get; set; }

        public DbSet<ActivityField> ActivityFields { get; set; }

        public DbSet<Connector> Connectors { get; set; }

        public DbSet<TaskOperation> TaskOperations { get; set; }

        public DbSet<Workflow> Workflows { get; set; }

        public DbSet<WorkflowTrace> WorkflowTraces { get; set; }

        public DbSet<DynamicParameter> DynamicParameters { get; set; }

        public DbSet<DynamicParameterValue> DynamicParametersValues { get; set; }

        public DbSet<WorkflowForm> StaticForms { get; set; }
    }
}
