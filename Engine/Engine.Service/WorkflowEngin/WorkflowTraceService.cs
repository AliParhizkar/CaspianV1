using Caspian.Common.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Caspian.Engine.Service
{
    public class WorkflowTraceService : SimpleService<WorkflowTrace>
    {
        public WorkflowTraceService(IServiceScope scope)
            :base(scope)
        {

        }

        async public Task<WorkflowTrace> Create(WorkflowTrace trace)
        {
            var connector = await new ConnectorService(ServiceScope).SingleAsync(trace.ConnectorId);
            if (connector.Activity.CategoryType != CategoryType.Start)
                throw new Exception("خطا:From activity must be start Category Type Activity");
            trace.TraceId = (await GetAll().MaxAsync(t => (int?)t.TraceId)).GetValueOrDefault(1);
            return trace;
        }

        async public Task<WorkflowTrace> GetWorkflowTrace(int activityId, int traceId, bool isUpdate)
        {
            var query = GetAll().Where(t => t.TraceId == traceId);
            if (isUpdate)
                query = query.Where(t => t.Connector.ActivityId == activityId);
            else
                query = query.Where(t => t.Connector.ToActivityId == activityId);
            var list = await query.ToListAsync();
            return list.Single(t => t.Id == list.Max(u => u.Id));
        }

        /// <summary>
        /// این متد کد کاربر ایجاد کننده گردش را برمی گرداند
        /// </summary>
        /// <param name="traceId"></param>
        /// <returns></returns>
        public int? GetWorkflowGeneratorId(int traceId)
        {
            return GetAll().Where(t => t.TraceId == traceId).OrderBy(t => t.Id).First().SenderId;
        }

        public IQueryable<WorkflowTrace> GetUserTrace(int userId, int ActivityId)
        {
            var query = GetAll();

            //var query1 = GetAll().Where(t => t.ReciverId == userId && (t.Connector.ToActivityId == ActivityId || t.Connector.ActivityId == ActivityId) && 
            //    t.Id == query.Where(u => u.TraceId == t.TraceId).Max(u => u.Id));
            return query.Where(t => (t.SenderId == userId && t.Connector.ActivityId == ActivityId) || t.ReciverId == userId && t.Connector.ToActivityId == ActivityId);
        }
    }
}
