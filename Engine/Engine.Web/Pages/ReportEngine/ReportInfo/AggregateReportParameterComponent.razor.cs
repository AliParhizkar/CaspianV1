using Caspian.UI;
using Caspian.Common;
using Caspian.Engine.Model;
using Caspian.Engine.Service;
using Caspian.Common.Extension;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Components;
using Castle.Components.DictionaryAdapter.Xml;

namespace Caspian.Engine.ReportGenerator
{
    public partial class AggregateReportParameterComponent:BasePage
    {
        Type type;
        Report report;
        IList<NodeView> source;
        TreeView<NodeView> tree;
        IList<AggregateReportGroupParameter> reportGroupparameters;
        IList<AggregateReportParameter> reportParameters;

        async Task SaveParameters()
        {
            var parameters = tree.GetSelectedItems();
            var list = new List<AggregateReportParameter>();
            foreach (var item in parameters )
            {
                int parameterId = 0;
                if (item.Parent == null)
                    parameterId = reportGroupparameters.Single(t => t.Path == item.Value).Id;
                else
                {
                    var parent = reportGroupparameters.Single(t => t.Path == item.Parent.Value);
                    int aggregateFunctionType = 0;
                    if (int.TryParse(item.Value, out aggregateFunctionType))
                        parameterId = parent.Parameters.Single(t => t.AggregateFunctionType.ConvertToInt() == aggregateFunctionType).Id;
                    else
                        parameterId = parent.Parameters.Single(t => t.Path == item.Value).Id;
                }
                list.Add(new AggregateReportParameter()
                {
                    AggregateReportGroupParameterId = parameterId,
                    ReportId = ReportId
                });
            }
            var changedEntities = new List<ChangedEntity<AggregateReportParameter>>();
            foreach (var item in list)
            {
                if (!reportParameters.Any(t => t.AggregateReportGroupParameterId == item.AggregateReportGroupParameterId))
                {
                    changedEntities.Add(new ChangedEntity<AggregateReportParameter>()
                    {
                        ChangeStatus = ChangeStatus.Added,
                        Entity = item
                    });
                }
            }
            foreach (var item in reportParameters)
            {
                if (!list.Any(t => t.AggregateReportGroupParameterId == item.AggregateReportGroupParameterId))
                {
                    changedEntities.Add(new ChangedEntity<AggregateReportParameter>()
                    {
                        ChangeStatus = ChangeStatus.Deleted,
                        Entity = item
                    });
                }
            }
            using var service = CreateScope().GetService<ReportService>();
            var old = await service.SingleAsync(ReportId);
            await service.UpdateDatabaseAsync(old, null, changedEntities);
            await service.SaveChangesAsync();
        }

        void NodeChanged(NodeView node)
        {
            if (node.Children != null)
            {
                foreach (var child in node.Children)
                    child.Selected = node.Selected;
            }
            var parent = node.Parent;
            if (parent != null)
            {
                var sibling = parent.Children;
                foreach (var item in sibling)
                {
                    if (IsIdentityNode(item))
                    {
                        item.Selected = sibling.Where(t => t != item).Any(t => t.Selected == true);
                        break;
                    }
                }
                if (sibling.All(t => t.Selected == true))
                    parent.Selected = true;
                else if (!sibling.Any(t => t.Selected == true))
                    parent.Selected = false;
                else
                    parent.Selected = null;
            }
        }

        bool IsIdentityNode(NodeView node)
        {
            var parent = node.Parent;
            if (parent == null || reportGroupparameters == null || int.TryParse(node.Value, out _))
                return false;
            var child = reportGroupparameters.Single(t => t.Path == parent.Value).Parameters.Single(t => t.Path == node.Value);
            return child?.AggregateParameterType == AggregateParameterType.Identity;
        }

        protected async override Task OnInitializedAsync()
        {
            using var scope = CreateScope();
            report = await scope.GetService<ReportService>().GetAll().Include(t => t.ReportGroup).SingleAsync(ReportId);
            reportGroupparameters = await scope.GetService<AggregateReportGroupParameterService>().GetAll().Where(t => t.ReportGroupId == report.ReportGroupId).ToListAsync();
            reportParameters = await scope.GetService<AggregateReportParameterService>().GetAll().Where(t => t.ReportId == ReportId).ToListAsync();
            source = reportGroupparameters.Where(t => t.ParentParameterId == null).Select(t => new NodeView()
            {
                Value = t.Path,
                Text = t.Allis,
                Collabsable = t.Parameters?.Any() == true,
                Expanded = t.Parameters?.Any() == true,
                Selectable = true,
                Selected = reportParameters.Any(u => u.AggregateReportGroupParameterId == t.Id),
                Children = t.Parameters == null ? null : t.Parameters.Select(u => new NodeView()
                {
                    Value = u.Path ?? Convert.ToInt32(u.AggregateFunctionType).ToString(),
                    Text = u.Allis,
                    Selected = reportParameters.Any(x => x.AggregateReportGroupParameterId == u.Id),
                    Collabsable = false,
                    Selectable = true,
                }).ToList()
            }).ToList();
            foreach (var node in source.Where(t => t.Children != null))
                foreach (var child in node.Children)
                {
                    child.Parent = node;
                    if (IsIdentityNode(child))
                    {
                        child.Disabled = true;
                        break;
                    }
                }
            await base.OnInitializedAsync();
        }

        [Parameter]
        public int ReportId { get; set; }
    }
}
