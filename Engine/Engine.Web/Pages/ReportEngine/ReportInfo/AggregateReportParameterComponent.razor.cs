using Caspian.UI;
using Caspian.Common;
using Caspian.Engine.Model;
using Caspian.Engine.Service;
using Caspian.Common.Extension;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Components;
using System.Diagnostics;

namespace Caspian.Engine.ReportGenerator
{
    public partial class AggregateReportParameterComponent:BasePage
    {
        Type type;
        Report report;
        IList<NodeView> source;
        TreeView<NodeView> tree;
        IList<AggregateReportGroupParameter> parameters;

        async Task SaveParameters()
        {
            var parameters = tree.GetSeletcedItems();

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
            if (parent == null || parameters == null || node.Value == null)
                return false;
            var child = parameters.Single(t => t.Path == parent.Value).Parameters.Single(t => t.Path == node.Value);
            return child?.AggregateParameterType == AggregateParameterType.Identity;
        }

        protected async override Task OnInitializedAsync()
        {
            using var scope = CreateScope();
            report = await scope.GetService<ReportService>().GetAll().Include(t => t.ReportGroup).SingleAsync(ReportId);
            var service = scope.GetService<AggregateReportGroupParameterService>();
            parameters = await service.GetAll().Where(t => t.ReportGroupId == report.ReportGroupId).ToListAsync();
            source = parameters.Where(t => t.ParentParameterId == null).Select(t => new NodeView()
            {
                Value = t.Path,
                Text = t.Allis,
                Collabsable = t.Parameters?.Any() == true,
                Expanded = t.Parameters?.Any() == true,
                Selectable = true,
                Children = t.Parameters == null ? null : t.Parameters.Select(u => new NodeView()
                {
                    Value = u.Path,
                    Text = u.Allis,
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
