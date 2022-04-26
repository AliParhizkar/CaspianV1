using Caspian.Common.Service;
using Caspian.Common.Extension;
using Microsoft.Extensions.DependencyInjection;

namespace Caspian.Engine.Service
{
    public class ActivityService : SimpleService<Activity>
    {
        public ActivityService(IServiceScope scope)
            :base(scope)
        {

        }

        public void RemoverWorkflowActivities(int workflowId)
        {
            var activities = GetAll().Where(t => t.WorkflowId == workflowId);
            RemoveRange(activities);
        }

        public IQueryable<Activity> GetActivities(int workflowId)
        {
            return GetAll().Where(t => t.WorkflowId == workflowId);
        }

        public Connector GetConnector(Activity activity, object data, string action)
        {
            var list = activity.OutConnectors.Where(t => t.Title == action).ToList();
            if (list.Count == 1)
                return list[0];
            foreach (var connector in list)
            {
                var value1 = Convert.ToDecimal(data.GetMyValue(connector.FieldName));
                if (Compare(connector.CompareType.Value, value1, connector.Value.Value))
                    return connector;
            }
            throw new Exception("خطا:در حالت " + activity.Title + " شروط انشعاب درست ثبت نشده اند");
        }

        private bool Compare(CompareType comapre, decimal value1, decimal value2)
        {
            switch(comapre)
            {
                case CompareType.Equal:
                    return value1 == value2;
                case CompareType.GreaterThan:
                    return value1 > value2;
                case CompareType.GreaterThanOrEqual:
                    return value1 >= value2;
                case CompareType.LessThan:
                    return value1 < value2;
                case CompareType.LessThanOrEqual:
                    return value1 <= value2;
                case CompareType.NotEqual:
                    return value1 != value2;
                default:
                    throw new NotImplementedException("");
            }
        }
    }

    public static class CompareTypeExtenssion
    {
        public static string GetMathSign(this CompareType compareType)
        {
            switch (compareType)
            {
                case CompareType.Equal:
                    return " = ";
                case CompareType.GreaterThan:
                    return " > ";
                case CompareType.GreaterThanOrEqual:
                    return " ≥ ";
                case CompareType.LessThan:
                    return " < ";
                case CompareType.LessThanOrEqual:
                    return " ≤ ";
                case CompareType.NotEqual:
                    return " ≠ ";
                default:
                    return "";
            }
        }
    }

}
