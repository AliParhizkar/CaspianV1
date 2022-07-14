using Caspian.Common.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Caspian.Engine.Service
{
    public class HtmlRowService : SimpleService<HtmlRow>, ISimpleService<HtmlRow>
    {
        public HtmlRowService(IServiceScope scope)
            :base(scope)
        {

        }

        public async Task<IList<HtmlRow>> GetRows(int formId)
        {
            return await GetAll().Where(t => t.WorkflowFormId == formId).Include(t => t.Columns).Include("Columns.Component")
                .Include("Columns.InnerRows").Include("Columns.Component.DynamicParameter")
                .Include("Columns.InnerRows.HtmlColumns").Include("Columns.InnerRows.HtmlColumns.Component")
                .Include("Columns.InnerRows.HtmlColumns.Component.DynamicParameter").ToListAsync();
        }
    }
}
