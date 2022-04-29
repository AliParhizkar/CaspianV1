using Caspian.UI;
using Main.Models;
using System.Linq;
using Caspian.Common;
using System.Xml.Linq;
using System.Reflection;
using Microsoft.JSInterop;
using Page.FormEngine.Models;
using Caspian.Engine.Service;
using Caspian.Common.Extension;
using Microsoft.AspNetCore.Components;

namespace Caspian.Engine.WorkFlowGenrator
{
    public partial class IndexComponent
    {
        string message;
        Activity Activity;
        WindowStatus Status;
        Connector Connector;
        WorkFlowModel workFlowModel;
        IList<TreeViewItem> FieldNodes;
        IList<TreeViewItem> DynamicFieldNodes;
        WorkflowWindowType? WorkflowWindowType;
        IList<Action> CheckMethodsList;
        protected override Task OnInitializedAsync()
        {
            using var scope = CreateScope();
            workFlowModel = new WorkFlowModel();
            workFlowModel.Activities = new ActivityService(scope).GetActivities(WorkflowId)
                    .Select(t => new
                    {
                        t.Id,
                        t.CategoryType,
                        t.Title,
                        t.ActorType,
                        t.Left,
                        t.Top,
                        t.Action.ClassName,
                        t.Action.MethodName,
                        t.Action.Namespace,
                        Fields = t.Fields.Select(u => new
                        {
                            u.FieldName,
                            u.ShowType
                        }),
                        DynamicFields = t.DynamicFields.Select(u => new
                        {
                            u.ControlId,
                            u.FormId,
                            u.ShowTime,
                            u.ShowType,
                            u.Title,
                        }),
                        t.Action.SystemActionType,
                        Actions = t.OutConnectors.Select(u => u.Title)
                    }).ToList().Select(t => new Activity()
                    {
                        CategoryType = t.CategoryType,
                        Left = t.Left,
                        Title = t.Title,
                        Top = t.Top,
                        Id = t.Id,
                        ActorType = t.ActorType,
                        OutConnectors = t.Actions.Select(u => new Connector()
                        {
                            Title = u
                        }).ToList(),
                        Fields = t.Fields.Select(u => new ActivityField()
                        {
                            FieldName = u.FieldName,
                            ShowType = u.ShowType
                        }).ToList(),
                        Action = new Caspian.Engine.Action()
                        {
                            ClassName = t.ClassName,
                            MethodName = t.MethodName,
                            Namespace = t.Namespace,
                            SystemActionType = t.SystemActionType
                        },
                        DynamicFields = t.DynamicFields.Select(u => new ActivityDynamicField()
                        {
                            FormId = u.FormId,
                            ControlId = u.ControlId,
                            ShowTime = u.ShowTime,
                            ShowType = u.ShowType,
                            Title = u.Title
                        }).ToList(),
                    }).Select(t => new ActivityModel(t)).ToList();
            workFlowModel.Connectors = new ConnectorService(scope).GetAll().Select(t => new
            {
                t.Id,
                t.ActivityId,
                t.PortType,
                t.Title,
                t.ToActivityId,
                t.ToPortType,
                t.CompareType,
                t.FieldName,
                t.CheckValidation,
                t.Value
            }).ToList().Select(t => new Connector()
            {
                ActivityId = t.ActivityId,
                Id = t.Id,
                PortType = t.PortType,
                Title = t.Title,
                ToActivityId = t.ToActivityId,
                ToPortType = t.ToPortType,
                CompareType = t.CompareType,
                FieldName = t.FieldName,
                CheckValidation = t.CheckValidation,
                Value = t.Value
            }).Select(t => new ConnectorModel(t)).ToList();
            return base.OnInitializedAsync();
        }

        [JSInvokable]
        public void ShowActivityProperty(Activity activity)
        {
            Activity = activity;
            Status = WindowStatus.Open;
            WorkflowWindowType = Main.Models.WorkflowWindowType.ActivityProperty;
            StateHasChanged();
        }

        [JSInvokable]
        public void ShowUserAndProperyRefrence(Connector connector)
        {
            Connector = connector;
            WorkflowWindowType = Main.Models.WorkflowWindowType.UserAndProperyRefrence;
            Status = WindowStatus.Open;
            StateHasChanged();
        }

        [JSInvokable]
        public void ShowMethodReturnRefrence(ConnectorModel connector)
        {
            WorkflowWindowType = Main.Models.WorkflowWindowType.MethodReturnRefrence;
            var action = connector.FromActivity.Action;
            Connector = new Connector()
            {
                Activity = new Activity()
                {
                    Action = new Caspian.Engine.Action()
                    {
                        Namespace = action.Namespace,
                        ClassName = action.ClassName,
                        MethodName = action.MethodName
                    }
                },
                CompareType = connector.CompareType,
                Value = connector.Value
            };
            Status = WindowStatus.Open;
            StateHasChanged();
        }

        [JSInvokable]
        public async Task ShowProcessAction(ActionModel model)
        {
            using var scope = ServiceScopeFactory.CreateScope();
            CheckMethodsList = (await new ActionService(scope).GetProcessAction(WorkflowId)).ToList();
            var index = 1;
            foreach (var action in CheckMethodsList)
            {
                action.Selected = model.Namespace == action.Namespace && model.ClassName == action.ClassName && model.MethodName == action.MethodName;
                action.Id = index;
                index++;
            }
            Status = WindowStatus.Open;
            WorkflowWindowType = Main.Models.WorkflowWindowType.ProcessMethod;
            StateHasChanged();
        }

        [JSInvokable]
        public async Task ActivityDynamicField(ActivityDynamicField[] fields)
        {
            using var scope = ServiceScopeFactory.CreateScope();
            var workflowService = new WorkflowService(scope);
            var wf = await workflowService.SingleAsync(WorkflowId);
            var forms = (await workflowService.SingleAsync(WorkflowId)).FormGroup.Forms
                .Where(t => t.FileName != null).Select(t => new
                {
                    t.Id,
                    t.Title,
                    t.FileName
                }).ToList();
            DynamicFieldNodes = new List<TreeViewItem>();
            foreach (var form in forms)
            {
                var item = new TreeViewItem()
                {
                    Text = form.Title,
                    Selectable = false,
                    Items = new List<TreeViewItem>(),
                    Expanded = true,
                };
                var path = Assembly.GetExecutingAssembly().GetMapPath() + "/data/Form/" + form.FileName + ".xml";
                var bond = new FormModel(XElement.Load(path)).Bond;
                var controls = new FormModel(XElement.Load(path)).Bond.Controls.Where(t => t.Id.HasValue);
                foreach (var control in controls.Where(t => !t.SpecialField.HasValue))
                {
                    var item1 = new TreeViewItem()
                    {
                        Text = control.Text,
                        Selectable = false,
                        Items = new List<TreeViewItem>(),
                        Expanded = true
                    };
                    item1.Items.Add(new TreeViewItem()
                    {
                        Text = "فعال",
                        Value = control.Id.ToString(),
                        Selectable = true,
                        Collabsable = false,
                        Selected = fields.Count() == 0 || fields.Any(t => t.ControlId == control.Id && t.ShowType == ShowType.Enable)
                    });
                    item1.Items.Add(new TreeViewItem()
                    {
                        Text = "غیرفعال",
                        Value = control.Id.ToString(),
                        Selectable = true,
                        Collabsable = false,
                        Selected = fields.Any(t => t.ControlId == control.Id && t.ShowType == ShowType.Disable)
                    });
                    item.Items.Add(item1);
                }
                foreach (var control in controls.Where(t => t.SpecialField.HasValue))
                {
                    var item1 = new TreeViewItem()
                    {
                        Text = control.Text,
                        Selectable = false
                    };
                    item1.Items.Add(new TreeViewItem()
                    {
                        Text = "قبل از ارجاع",
                        Value = control.Id.ToString(),
                        Selectable = true,
                        Collabsable = false,
                        Selected = fields.Any(t => t.ControlId == control.Id && t.ShowTime == ShowTime.BeforRef)
                    });
                    item1.Items.Add(new TreeViewItem()
                    {
                        Text = "بعد از ارجاع",
                        Value = control.Id.ToString(),
                        Selectable = true,
                        Collabsable = false,
                        Selected = fields.Count() == 0 || fields.Any(t => t.ControlId == control.Id && t.ShowTime == ShowTime.OfterRef)
                    });
                    item.Items.Add(item1);
                }
                if (item.Items.Count > 0)
                    DynamicFieldNodes.Add(item);
            }

            Status = WindowStatus.Open;
            WorkflowWindowType = Main.Models.WorkflowWindowType.DynamicFields;
            StateHasChanged();
        }

        [JSInvokable]
        public async Task CheckMethods(ActionModel model)
        {
            using var scope = ServiceScopeFactory.CreateScope();
            CheckMethodsList = (await new ActionService(scope).GetCheckingAction(WorkflowId)).ToList();
            var index = 1;
            foreach (var action in CheckMethodsList)
            {
                action.Selected = model.Namespace == action.Namespace && model.ClassName == action.ClassName && model.MethodName == action.MethodName;
                action.Id = index;
                index++;
            }
            Status = WindowStatus.Open;
            WorkflowWindowType = Main.Models.WorkflowWindowType.CheckMethod;
            StateHasChanged();
        }

        [JSInvokable]
        public async Task Save(string data)
        {
            using var scope = CreateScope();
            var workflow = Newtonsoft.Json.JsonConvert.DeserializeObject<WorkFlowModel>(data);
            var activities = workflow.Activities.Select(t => t.GetActivity()).ToList();
            var fields = new List<ActivityField>();
            var connectors = workflow.Connectors.Select(t => t.GetConnector()).ToList();
            new ConnectorService(scope).RemoverWorkflowConnectors(WorkflowId);
            new ActivityService(scope).RemoverWorkflowActivities(WorkflowId);
            var activityFieldService = new ActivityFieldService(scope);
            activityFieldService.RemoverWorkflowFields(WorkflowId);
            var actionService = new ActionService(scope);
            actionService.RemoverWorkflowActions(WorkflowId);
            await actionService.SaveChangesAsync();
            var maxId = new ActivityService(scope).GetAll().Max(t => (int?)t.Id).GetValueOrDefault() + 1;
            var index = maxId;
            foreach (var activity in activities)
            {
                if (activity.Fields != null)
                {
                    foreach(var field in activity.Fields)
                    {
                        field.ActivityId = activity.Id;
                        fields.Add(field);
                    }
                }
                activity.Fields = null;
                foreach (var con in connectors)
                {
                    if (con.ActivityId == activity.Id)
                        con.ActivityId = index;
                    if (con.ToActivityId == activity.Id)
                        con.ToActivityId = index;
                }
                if (activity.Action != null)
                {
                    activity.ActionId = index;
                    activity.Action.Id = index;
                    activity.Action.Activity = null;
                }
                activity.Id = index; 
                activity.WorkflowId = WorkflowId;
                index++;
            }
            await new ActivityService(scope).AddRangeAsync(activities.Where(t => t.Id == 0));
            new ActivityService(scope).UpdateRange(activities.Where(t => t.Id > 0));
            //maxId = new ConnectorService(Context).GetAll().Max(t => (int?)t.Id).GetValueOrDefault() + 1;
            //index = maxId;
            //foreach (var connector in connectors)
            //{
            //    connector.Id = index;
            //    index++;
            //}
            await new ConnectorService(scope).AddRangeAsync(connectors);
            await actionService.SaveChangesAsync();
            //maxId = new ActivityFieldService(Context).GetAll().Max(t => (int?)t.Id).GetValueOrDefault() + 1;
            //index = maxId;
            //foreach(var field in fields)
            //{
            //    field.Id = index;
            //    index++;
            //}
            await activityFieldService.AddRangeAsync(fields);
            await activityFieldService.SaveChangesAsync();
            message = "ثبت با موفقیت انجام شد";
            StateHasChanged();
        }

        [JSInvokable]
        public void ActivityField(Activity activity)
        {
            Activity = activity;
            Status = WindowStatus.Open;
            WorkflowWindowType = Main.Models.WorkflowWindowType.ActivityField;
            StateHasChanged();
        }

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                var json = workFlowModel.GetJson();
                await JsRuntime.InvokeVoidAsync("$.workflow.init", DotNetObjectReference.Create(this), json);
            }
            if (message.HasValue())
            {
                await JsRuntime.InvokeVoidAsync("$.telerik.showMessage", message);
                message = null;
            }
            await base.OnAfterRenderAsync(firstRender);
        }

        [Parameter]
        public int WorkflowId { get; set; } = 1;

    }
}
