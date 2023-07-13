using System.Text;
using Caspian.Common;
using Caspian.Common.Service;
using Caspian.Common.Extension;
using Microsoft.EntityFrameworkCore;

namespace Caspian.Engine.Service
{
    public class WorkflowFormService : BaseService<Engine.WorkflowForm>, IBaseService<Engine.WorkflowForm>
    {
        public WorkflowFormService(IServiceProvider provider)
            :base(provider)
        {
            RuleFor(t => t.Title).Required().UniqAsync("فرمی با این عنوان در سیستم ثبت شده است");
            RuleFor(t => t.ColumnCount).CustomValue(t => t < 1, "حداقل یک ستون باید وجود داشته باشد")
                .CustomValue(t => t > 4, "جداکثر چهار ستون می تواند وجود داشته باشد");
            RuleFor(t => t.Name).UniqAsync("فرمی با این نام در سیستم ثبت شده است").CustomValue(t => t.IsValidIdentifire(), "برای تعریف کلاس فقط از کاراکترهای لاتین و اعداد استفاده نمایید.");
        }

        public async Task<WorkflowForm> GetWorkflowForm(int workflowFormId)
        {
            var form = await GetAll().Include(t => t.Rows)
                .Include(t => t.WorkflowGroup).Include("Rows.Columns")
                .Include(t => t.DataModel).Include(t => t.DataModel.Fields)
                .Include("Rows.Columns.Component")
                .Include("Rows.Columns.Component.LookupType")
                .Include("Rows.Columns.Component.DataModelField.DataModelOptions")
                .Include("Rows.Columns.Component.DataModelField")
                .Include("Rows.Columns.Component.DataModelField.EntityType")
                .Include("Rows.Columns.InnerRows").Include("Rows.Columns.InnerRows.HtmlColumns")
                .Include("Rows.Columns.InnerRows.HtmlColumns.Component")
                .Include("Rows.Columns.InnerRows.HtmlColumns.Component.LookupType")
                .Include("Rows.Columns.InnerRows.HtmlColumns.Component.DataModelField")
                .Include("Rows.Columns.InnerRows.HtmlColumns.Component.DataModelField.DataModelOptions")
                .Include("Rows.Columns.InnerRows.HtmlColumns.Component.DataModelField.EntityType")
                .SingleAsync(workflowFormId);
            return form;
        }
    }
}
