using Caspian.Common;
using Caspian.Engine;
using Syncfusion.Blazor.Diagram;

namespace Engine.Web.Services
{
    public static class WorkflowExtension
    {
        public static Connector ConvertToBpmnConnector(this NodeConnector nodeConnector, IDictionary<string, ConnectorData> connectors)
        {
            var data = connectors.ContainsKey("Connector" + nodeConnector.Id) ? connectors["Connector" + nodeConnector.Id] :
                new ConnectorData();
            data.Title = nodeConnector.Title;
            data.Description = nodeConnector.Description;
            connectors["Connector" + nodeConnector.Id] = data;
            
            return new Connector()
            {
                Annotations = new DiagramObjectCollection<PathAnnotation>()
                {
                    new PathAnnotation()
                    {
                        Content = nodeConnector.Title,
                        SegmentAngle = true,
                        Style = new TextStyle()
                        {
                            FontSize= 14,
                            TextAlign= TextAlign.Right,
                        },
                        Alignment = AnnotationAlignment.Before,
                        HorizontalAlignment = HorizontalAlignment.Right,
                    }
                },
                ID = "Connector" + nodeConnector.Id,
                Shape = new BpmnFlow()
                {
                    Flow = BpmnFlowType.SequenceFlow
                },
                CanAutoLayout = true,
                Type = ConnectorSegmentType.Straight,
                SourceID = "CNode" + nodeConnector.ActivityId,
                TargetID = "CNode" + nodeConnector.ToActivityId
            };
        }

        public static NodeConnector ConvertToNodeConnector(this Connector connector, IDictionary<string, ConnectorData> connectorsData, IList<Node> nodes) 
        {
            var con = new NodeConnector();
            var data = connectorsData[connector.ID];
            con.Title = data?.Title;
            con.Description = data?.Description;
            if (connector.ID.StartsWith("Connector") ) 
            {
                var id = connector.ID.Substring(9);
                con.Id = Convert.ToInt32(id);
            }
            if (connector.SourceID?.StartsWith("CNode") == true)
            {
                var id = nodes.Single(t => t.ID == connector.SourceID.ToString()).ID;
                con.ActivityId = Convert.ToInt32(id.Substring(5));
            }
            if (connector.TargetID?.StartsWith("CNode") == true)
            {
                var id = nodes.Single(t => t.ID == connector.TargetID.ToString()).ID;
                con.ToActivityId = Convert.ToInt32(id.Substring(5));
            }
            return con;
        }

        public static Node ConvertToBpmNode(this Activity activity)
        {
            var node = new Node()
            {
                OffsetX = activity.Left,
                ID = "CNode" + activity.Id.ToString(),
                OffsetY = activity.Top,
                Width = 40,
                Height = 40,
                Data = new NodeData()
                {
                    Title = activity.Title,
                    Description = activity.Description,
                    WorkflowFormId = activity.WorkflowFormId,
                },
                Ports = new DiagramObjectCollection<PointPort>
                {
                    CreatePort(0.5, 0.5),
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
                        TaskType = activity.TaskType.ConvertToBpmnTask(),
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

        static PointPort CreatePort(double x, double y)
        {
            return new PointPort()
            {
                Height = 12,
                Width = 12,
                Shape = PortShapes.Circle,
                Offset = new DiagramPoint(x, y),
                Visibility = PortVisibility.Hover,
                
                Style = new ShapeStyle()
                {
                    Fill = "#8CC63F",
                },
                Constraints = PortConstraints.Default | PortConstraints.Draw
            };
        }

        public static Activity ConvertToActivity(this Node node)
        {
            if (node == null) 
                return null;
            var data = node.Data as NodeData;
            var activity = new Activity()
            {
                ActivityType = node.Shape.ConvertToActivityType(),
                TaskType = node.Shape.ConvertToTaskType(),
                GatewayType = node.Shape.ConvertToGatewayType(),
                EventTriggerType = node.Shape.ConvertToTriggerType(),
                Left = node.OffsetX,
                Top = node.OffsetY,
                Title = data.Title,
                WorkflowFormId = data.WorkflowFormId,
                Description = data.Description
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
