using Caspian.Engine;
using Syncfusion.Blazor.Diagram;

namespace Engine.Web.Services
{
    public static class WorkflowExtension
    {
        public static Node ConvertToBpmNode(this Activity activity)
        {
            var node = new Node();
            node.ID = activity.Id.ToString();
            node.OffsetX = activity.Left; 
            node.OffsetY = activity.Top;
            node.Width = 40;
            node.Height = 40;
            node.Data = new object()
            {

            };
            node.Annotations = new DiagramObjectCollection<ShapeAnnotation>
                    {
                        new ShapeAnnotation()
                        {
                            Content = activity.Title,
                            HorizontalAlignment = HorizontalAlignment.Right,
                            Offset = new DiagramPoint(0.95, 0.5),
                        }
                    };
            switch (activity.ActivityType)
            {
                case ActivityType.Start:
                    node.Shape = new BpmnEvent() { EventType = BpmnEventType.Start };
                    break;
                case ActivityType.End: 
                    node.Shape = new BpmnEvent() { EventType = BpmnEventType.End };
                    break;
                case ActivityType.Task:
                    node.Shape = new BpmnActivity()
                    {
                        ActivityType = BpmnActivityType.Task,
                        TaskType = activity.TaskType.ConvertToBpmnTask()
                    };
                    break;
                case ActivityType.Gateway:
                    node.Shape = new BpmnGateway()
                    {
                        GatewayType = activity.GatewayType.ConvertToBpmnGateway()
                    };
                    break;
                case ActivityType.Event:
                    node.Shape = new BpmnEvent()
                    {
                        EventType = BpmnEventType.Intermediate,
                        Trigger = activity.EventTriggerType.ConvertToBpmnEventTrigger()
                    };
                    break;
                default:
                    throw new NotImplementedException();
            }
            return node;
        }

        public static Activity ConvertToActivity(this Node node)
        {
            if (node == null) 
                return null;
            var activity = new Activity()
            {
                Title = node.Annotations.Count == 0 ? "" : node.Annotations[0].Content,
                ActivityType = node.Shape.ConvertToActivityType(),
                TaskType = node.Shape.ConvertToTaskType(),
                GatewayType = node.Shape.ConvertToGatewayType(),
                EventTriggerType = node.Shape.ConvertToTriggerType(),
                Left = node.OffsetX,
                Top = node.OffsetY
            };
            return activity;
        }

        public static EventTriggerType? ConvertToTriggerType(this Shape shape)
        {
            var activity = shape as BpmnEvent;
            if (activity == null || activity.Trigger == BpmnEventTrigger.None)
                return null;
            switch (activity.Trigger)
            {
                case BpmnEventTrigger.Signal: return EventTriggerType.Signal;
                case BpmnEventTrigger.Message: return EventTriggerType.Message;
                case BpmnEventTrigger.Timer: return EventTriggerType.Timer;
                case BpmnEventTrigger.Terminate: return EventTriggerType.Terminate;
                case BpmnEventTrigger.Error: return EventTriggerType.Error;
                case BpmnEventTrigger.Conditional: return EventTriggerType.Conditional;
                case BpmnEventTrigger.Multiple: return EventTriggerType.Multiple;
                case BpmnEventTrigger.Parallel: return EventTriggerType.Parallel;
                case BpmnEventTrigger.Cancel: return EventTriggerType.Cancel;
                default: throw new NotImplementedException();
            }
        }

        public static GatewayType? ConvertToGatewayType(this Shape shape)
        {
            var activity = shape as BpmnGateway;
            if (activity == null || activity.GatewayType == BpmnGatewayType.None)
                return null;
            switch(activity.GatewayType)
            {
                case BpmnGatewayType.Exclusive: return GatewayType.Exclusive;
                case BpmnGatewayType.EventBased: return GatewayType.Eventbased;
                case BpmnGatewayType.Parallel: return GatewayType.Parallel;
                case BpmnGatewayType.ParallelEventBased: return GatewayType.ParallelEventbased;
                case BpmnGatewayType.Inclusive: return GatewayType.Inclusive;
                case BpmnGatewayType.Complex: return GatewayType.Complex;
                default: throw new NotImplementedException();
            }
        }

        public static TaskType? ConvertToTaskType(this Shape shape)
        {
            var activity = shape as BpmnActivity;
            if (activity == null || activity.TaskType == BpmnTaskType.None)
                return null;
            switch(activity.TaskType)
            {
                case BpmnTaskType.Manual: return TaskType.Manual;
                case BpmnTaskType.BusinessRule: return TaskType.BusinessRule;
                case BpmnTaskType.User: return TaskType.User;
                case BpmnTaskType.Send: return TaskType.Send;
                case BpmnTaskType.Receive: return TaskType.Receive;
                case BpmnTaskType.Service: return TaskType.Service;
                case BpmnTaskType.Script: return TaskType.Script;
                default: throw new NotImplementedException();
            }
        }

        public static ActivityType ConvertToActivityType(this Shape shape)
        {
            if (shape is BpmnActivity)
                return ActivityType.Task;
            if (shape is BpmnEvent)
            {
                var eventType = (shape as BpmnEvent).EventType;
                switch(eventType)
                {
                    case BpmnEventType.Start: return ActivityType.Start;
                    case BpmnEventType.End: return ActivityType.End;
                    case BpmnEventType.Intermediate: return ActivityType.Event;
                    default: throw new NotImplementedException();
                }
            }
            if (shape is BpmnGateway)
                return ActivityType.Gateway;
            throw new NotImplementedException();
        }

        public static BpmnEventTrigger ConvertToBpmnEventTrigger(this EventTriggerType? triggerType)
        {
            switch (triggerType)
            {
                case EventTriggerType.Signal: return BpmnEventTrigger.Signal;
                case EventTriggerType.Message: return BpmnEventTrigger.Message;
                case EventTriggerType.Timer: return BpmnEventTrigger.Timer;
                case EventTriggerType.Terminate: return BpmnEventTrigger.Terminate;
                case EventTriggerType.Error: return BpmnEventTrigger.Error;
                case EventTriggerType.Conditional: return BpmnEventTrigger.Conditional;
                case EventTriggerType.Multiple: return BpmnEventTrigger.Multiple;
                case EventTriggerType.Parallel: return BpmnEventTrigger.Parallel;
                case EventTriggerType.Cancel: return BpmnEventTrigger.Cancel;
                case null: return BpmnEventTrigger.None;
                default: throw new NotImplementedException();
            }
        }

        public static BpmnGatewayType ConvertToBpmnGateway(this GatewayType? gateway)
        {
            switch (gateway)
            {
                case GatewayType.Exclusive: return BpmnGatewayType.Exclusive;
                case GatewayType.Eventbased: return BpmnGatewayType.EventBased;
                case GatewayType.Parallel: return BpmnGatewayType.Parallel;
                case GatewayType.ParallelEventbased: return BpmnGatewayType.ParallelEventBased;
                case GatewayType.Inclusive: return BpmnGatewayType.Inclusive;
                case GatewayType.Complex: return BpmnGatewayType.Complex;
                case null: return BpmnGatewayType.None;
                default: throw new NotImplementedException();
            }
        }

        public static BpmnTaskType ConvertToBpmnTask(this TaskType? taskType)
        {
            switch(taskType)
            {
                case TaskType.Script: return BpmnTaskType.Script;
                case TaskType.Send: return BpmnTaskType.Send;
                case TaskType.Service: return BpmnTaskType.Service;
                case TaskType.Receive: return BpmnTaskType.Receive;
                case TaskType.User: return BpmnTaskType.User;
                case TaskType.BusinessRule: return BpmnTaskType.BusinessRule;
                case TaskType.Manual: return BpmnTaskType.Manual;
                case null: return BpmnTaskType.None;
                default:throw new NotImplementedException();
            }
        }

    }
}
