using Caspian.UI;
using ReportUiModels;
using Caspian.Common;
using System.Reflection;
using Caspian.Engine.Model;
using Caspian.Engine.Service;
using Caspian.Common.Extension;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.Engine.ReportGenerator
{
    public partial class AggregateReportGroupParameterComponent
    {
        string allis;
        IList<NodeView> nodes;
        IList<NodeView> source;
        ReportGroup reportGroup;
        IList<AggregateReportGroupParameter> parameters;
        AggregateReportGroupParameter selectedParameter;
        WindowStatus windowStatus;
        IList<AggregateReportGroupParameter> removedParameters;
        Type reportType;
        void OpenForm(NodeView node)
        {
            windowStatus = WindowStatus.Open;
            if (node.Parent ==  null) 
                selectedParameter = parameters.Single(t => t.Path == node.Value);
            else
                selectedParameter = parameters.Single(t => t.Path == node.Parent.Value).Parameters.Single(t => t.Path == node.Value);
            allis = node.Text;
        }

        void UpdateAllis(string path, string allis)
        {
            var message = ""; ;
            if (!allis.HasValue())
                message = "لطفا اسم مستعار پارامتر را مشخص نمایید";
            else
            {
                var multiple = parameters.Any(t => t.Path != path && t.Allis == allis) || parameters.Any(t => t.Parameters.Any(u => u.Path != path && u.Allis == allis));
                if (multiple)
                    message = "پارامتر دیگر دارای این اسم مستعار می باشد";
            }
            if (message.HasValue())
                ShowMessage(message);
            else
            {
                selectedParameter.Allis = allis;
                if (selectedParameter.Parameters != null)
                {
                    var identityNode = selectedParameter.Parameters.SingleOrDefault(u => u.AggregateParameterType == AggregateParameterType.Identity);
                    if (identityNode != null)
                        identityNode.Allis = $"شناسه {selectedParameter.Allis}";
                }
                windowStatus = WindowStatus.Close;
                CreateTree();
            }
        }
        
        async Task UpsertAll()
        {
            using var service = CreateScope().GetService<AggregateReportGroupParameterService>();
            foreach(var param in parameters.Union(removedParameters.Where(t => t.AggregateParameterType == AggregateParameterType.Parent)))
            {
                if (param.Id == 0)
                    await service.AddAsync(param);
                else
                {
                    var list = param.Parameters.Select(t => new ChangedEntity<AggregateReportGroupParameter>()
                    {
                        ChangeStatus = t.Id == 0 ? ChangeStatus.Added : ChangeStatus.Updated,
                        Entity = t
                    }).ToList();
                    list.AddRange(removedParameters.Where(t => t.ParentParameterId == param.Id).Select(t => new ChangedEntity<AggregateReportGroupParameter>() 
                    {
                        ChangeStatus = ChangeStatus.Deleted,
                        Entity = t
                    }));
                    await service.UpdateDatabaseAsync(param, list);
                }
                await service.SaveChangesAsync();
            }
            foreach (var param in removedParameters.Where(t => t.AggregateParameterType == AggregateParameterType.Parent))
            {
                var old = await service.SingleAsync(param.Id);
                await service.RemoveAsync(old);
                await service.SaveChangesAsync();
            }
            ShowMessage("ثبت با موفقیت انجام شد.");
            await ReLoadData();
        }

        bool IsIdentityNode(NodeView node)
        {
            var parent = node.Parent;
            if (parent == null || parameters == null)
                return false;
            var child = parameters.Single(t => t.Path == parent.Value).Parameters?.FirstOrDefault(t => t.Path == node.Value);
            return child?.AggregateParameterType == AggregateParameterType.Identity;
        }

        void AddSelectNodeToTree(NodeView node)
        {
            var parent = parameters.SingleOrDefault(t => t.Path == node.Parent.Value);
            if (parent == null)
            {
                parent = new AggregateReportGroupParameter()
                {
                    AggregateParameterType = AggregateParameterType.Parent,
                    Path = node.Parent.Value,
                    Allis = node.Parent.Text,
                    ReportGroupId = GroupId,
                    Parameters = new List<AggregateReportGroupParameter>()
                };
                parameters.Add(parent);
            }
            parent.Parameters.Add(new AggregateReportGroupParameter()
            {
                AggregateParameterType = AggregateParameterType.Selecting,
                Path = node.Value,
                Allis = node.Text,
                ReportGroupId = GroupId,
                ParentParameterId = parent.Id > 0 ? parent.Id : null
            });
            if (!parent.Parameters.Any(t => t.AggregateParameterType == AggregateParameterType.Identity))
            {
                var propertyInfo = reportType.GetMyProperty(parent.Path);
                var fKey = propertyInfo.GetCustomAttribute<ForeignKeyAttribute>().Name;
                var path = parent.Path;
                var lastIndex = path.LastIndexOf('.');
                if (lastIndex > 0)
                    path = path.Substring(0, lastIndex);
                path += '.' + fKey;
                parent.Parameters.Insert(0, new AggregateReportGroupParameter()
                {
                    AggregateParameterType = AggregateParameterType.Identity,
                    Path = path,
                    Allis = $"شناسه {parent.Allis}",
                    ReportGroupId = GroupId,
                });
            }
        }

        void UpdateNodesData(NodeView node)
        {
            if (node.Selected == true)
            {
                if (node.Parent == null)
                {
                    var info = reportType.GetMyProperty(node.Value);
                    var type = info.PropertyType.GetUnderlyingType();
                    var parameter = new AggregateReportGroupParameter()
                    {
                        Allis = node.Text,
                        ReportGroupId = GroupId,
                        Path = node.Value
                    };
                    if (type == typeof(DateTime))
                    {
                        parameter.AggregateParameterType = AggregateParameterType.Parent;
                        string[] dateFields = null;
                        if (info.DeclaringType.GetProperties().Any(t => t.PropertyType == typeof(PersianDateTable) && t.GetCustomAttribute<ForeignKeyAttribute>()?.Name == info.Name))
                            dateFields = ReportTree.TotalDateFields;
                        else
                            dateFields = ReportTree.DateFields;
                        parameter.Parameters = dateFields.Select(t => new AggregateReportGroupParameter()
                        {
                            AggregateParameterType = AggregateParameterType.Grouping,

                        }).ToList();
                    }
                    else
                    {
                        parameter.AggregateParameterType = AggregateParameterType.Grouping;
                    }
                    parameters.Add(parameter);
                }
                else if (node.Parent != null)
                    AddSelectNodeToTree(node);
            }
            else
            {
                if (node.Parent == null)
                {
                    var old = parameters.Single(t => t.Path == node.Value);
                    if (old.Id > 0)
                        removedParameters.Add(old);
                    parameters.Remove(old);
                }
                else
                {
                    var parent = parameters.Single(t => t.Path == node.Parent.Value);
                    var old = parent.Parameters.Single(t => t.Path == node.Value);
                    if (old.Id > 0)
                        removedParameters.Add(old);
                    parent.Parameters.Remove(old);
                    if (!parent.Parameters.Any(u => u.AggregateParameterType != AggregateParameterType.Identity))
                    {
                        parameters.Remove(parent);
                        if (parent.Id > 0)
                            removedParameters.Add(parent);
                        var identityNode = parent.Parameters[0];
                        parent.Parameters.RemoveAt(0);
                        if (identityNode.Id > 0)
                            removedParameters.Add(identityNode);
                    }
                }
            }
            CreateTree();
        }

        void CreateTree()
        {
            source = parameters.Select(t => new NodeView()
            {
                Value = t.Path,
                Text = t.Allis,
                Collabsable = t.Parameters?.Any() == true,
                Expanded = t.Parameters?.Any() == true,
                Children = t.Parameters == null ? null : t.Parameters.Select(u => new NodeView()
                {
                    Value = u.Path,
                    Text = u.Allis,
                    Collabsable = false
                }).ToList()
            }).ToList();
        }

        async Task ReLoadData()
        {
            removedParameters = new List<AggregateReportGroupParameter>();
            using var service = CreateScope().GetService<ReportGroupService>();
            reportGroup = await service.GetAll().Include(t => t.AggregateReportGroupParameters).SingleAsync(GroupId);
            parameters = reportGroup.AggregateReportGroupParameters.Where(t => t.ParentParameterId == null).ToList();
            CreateTree();
            reportType = new AssemblyInfo().GetReturnType(reportGroup);
            nodes = new ReportTree().CreateNodesForGroupBySelect(reportType);
            foreach (var node in nodes)
            {
                var parameter = parameters.SingleOrDefault(t => t.Path == node.Value);
                if (parameter != null)
                {
                    if (node.Children == null)
                        node.Selected = true;
                    else
                    {
                        foreach (var child in node.Children)
                            child.Selected = parameter.Parameters?.Any(u => u.Path == child.Value) == true;
                    }
                }
            }
        }

        protected override async Task OnInitializedAsync()
        {
            await ReLoadData();
            await base.OnInitializedAsync();
        }

        [Parameter]
        public int GroupId { get; set; }
    }
}
