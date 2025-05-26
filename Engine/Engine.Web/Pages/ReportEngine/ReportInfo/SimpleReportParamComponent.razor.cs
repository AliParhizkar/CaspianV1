using Caspian.UI;
using Caspian.Common;
using System.Reflection;
using Caspian.Engine.Model;
using Caspian.Engine.Service;
using Caspian.Common.Extension;
using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Caspian.Engine.ReportGenerator
{
    public partial class SimpleReportParamComponent: UI.BasePage
    {
        Report report;
        Type reportType;
        CaspianForm<Report> form;
        IList<int> selectedParameters;
        IList<ReportParam> removedParams;
        IList<SelectListItem> dataLevels;

        async Task OnChange(ChangedEntity<int> parameter)
        {
            if (parameter.ChangeStatus == ChangeStatus.Added)
            {
                var old = removedParams.SingleOrDefault(t => t.ReportGroupParameterId == parameter.Entity);
                if (old == null)
                {
                    using var service = CreateScope().GetService<ReportGroupParameterService>();
                    var param = await service.SingleAsync(parameter.Entity);
                    old = new ReportParam()
                    {
                        ReportId = ReportId,
                        ReportGroupParameter = param,
                        ReportGroupParameterId = param.Id,
                        DataLevel = 1
                    };
                }
                else
                    removedParams.Remove(old);
                report.ReportParams.Add(old);
            }
            else
            {
                var old = report.ReportParams.Single(t => t.ReportGroupParameterId == parameter.Entity);
                report.ReportParams.Remove(old);
                if (old.Id > 0)
                    removedParams.Add(old);
            }
        }

        async Task LoadData()
        {
            removedParams = new List<ReportParam>();
            using var service = CreateScope().GetService<ReportParamService>();
            report.ReportParams = await service.GetAll().Include(t => t.ReportGroupParameter).Where(t => t.ReportId == ReportId).ToListAsync();
            selectedParameters = report.ReportParams.Select(t => t.ReportGroupParameterId).ToList();
        }

        protected override async Task OnInitializedAsync()
        {
            dataLevels = SelectListItem.CreateList("سطح اول", "سطح دوم", "سطح سوم");
            using var service = CreateScope().GetService<ReportService>();
            report = await service.GetAll().Include(t => t.ReportGroup).SingleAsync(ReportId);
            reportType = new AssemblyInfo().GetReturnType(report.ReportGroup);
            await LoadData();
            await base.OnInitializedAsync();
        }

        async Task<IList<ChangedEntity<ReportParam>>> AddKeyToDataLevel(IList<ChangedEntity<ReportParam>> list, byte dataLevel)
        {
            if (report.ReportParams.Any(t => t.DataLevel == dataLevel && !t.ReportGroupParameter.IsKey))
            {
                var key = await GetKey(report.ReportParams.Where(t => t.DataLevel == dataLevel && !t.ReportGroupParameter.IsKey).ToList());
                if (!report.ReportParams.Any(t => t.ReportGroupParameterId == key))
                {
                    list.Add(new ChangedEntity<ReportParam>()
                    {
                        ChangeStatus = ChangeStatus.Added,
                        Entity = new ReportParam()
                        {
                            DataLevel = dataLevel,
                            ReportGroupParameterId = key,
                            ReportId = report.Id,
                        }
                    });
                }
                var oldKey = report.ReportParams.SingleOrDefault(t => t.ReportGroupParameterId != key && t.DataLevel == dataLevel && t.Id > 0 && t.ReportGroupParameter.IsKey);
                if (oldKey != null)
                {
                    list = list.Where(t => t.Entity.Id != oldKey.Id).ToList();
                    list.Add(new ChangedEntity<ReportParam>()
                    {
                        ChangeStatus = ChangeStatus.Deleted,
                        Entity = oldKey
                    });
                }
            }
            return list;
        }

        async Task SaveData()
        {
            using var scope = CreateScope();
            var service = scope.GetService<ReportService>();
            var old = await service.SingleAsync(ReportId);
            var list = report.ReportParams.Select(t => new ChangedEntity<ReportParam>
            {
                ChangeStatus = t.Id == 0 ? ChangeStatus.Added : ChangeStatus.Updated,
                Entity = t
            }).ToList();
            try
            {
                if (report.ReportType >= ReportType.TowLevels)
                    list = (await AddKeyToDataLevel(list, 2)).ToList();
                if (report.ReportType == ReportType.ThirdLevels)
                    list = (await AddKeyToDataLevel(list, 3)).ToList();
            }
            catch (CaspianException ex)
            {
                ShowMessage(ex.Message);
                return;
            }
            list.AddRange(removedParams.Select(t => new ChangedEntity<ReportParam>()
            {
                ChangeStatus = ChangeStatus.Deleted,
                Entity = t
            }));
            service.SetChangedEntities(list);
            await service.UpdateAsync(old);
            await service.SaveChangesAsync();
            await LoadData();
            ShowMessage("ثبت با موفقیت انجام شد.");
        }

        /// <summary>
        /// This Method Find parameter that is Key parameter for multi Level reports
        /// </summary>
        async Task<int> GetKey(IList<ReportParam> reportParams)
        {
            var service = CreateScope().GetService<ReportGroupParameterService>();
            var keies = await service.GetAll().Where(t => t.ReportGroupId == report.ReportGroupId && t.IsKey).ToListAsync();
            var list = new List<string>();
            var entityPathList = new List<string>();
            foreach (var param in reportParams)
            {
                var key = GetKeyEntityPath(param.ReportGroupParameter.PropertyPath);
                if (!list.Contains(key))
                {
                    list.Add(key);
                    entityPathList.Add(GetEntityPath(param.ReportGroupParameter.PropertyPath));
                }
            }
            entityPathList = entityPathList.OrderBy(t => t.Split('.').Length).ToList();
            var first = entityPathList.First();
            foreach (var entityPath in entityPathList)
                if (!entityPath.StartsWith(first))
                    throw new CaspianException($"بیش از یک نوع گروه بندی تعریف شده است:\n {first}, {entityPath}");
            var firstKey = list.OrderBy(t => t.Split('.').Length).First();
            return keies.Single(t => t.PropertyPath == firstKey).Id;
        }

        string GetEntityPath(string fullPath)
        {
            var type = reportType;
            string str = null;
            foreach (var section in fullPath.Split('.'))
            {
                var info = type.GetProperty(section);
                if (info.GetCustomAttribute<ForeignKeyAttribute>() == null)
                    break;
                else
                {
                    if (str != null)
                        str += '.';
                    str += info.Name;
                }
                type = info.PropertyType;
            }
            return str;
        }

        string GetKeyEntityPath(string fullPath)
        {
            var str = GetEntityPath(fullPath);
            var type = reportType;
            var name = type.GetMyProperty(str).GetCustomAttribute<ForeignKeyAttribute>().Name;
            var index = str.LastIndexOf('.');
            if (index > 0)
                return $"{str.Substring(0, index)}.{name}";
            return name;
        }

        [Parameter]
        public int ReportId { get; set; }

        byte MaxdataLevel(string path)
        {
            byte level = 1;
            var type = reportType;
            var qqq = report.ReportType.ConvertToInt().Value - 1;
            foreach (var section in path.Split('.'))
            {
                var info = type.GetProperty(section);
                if (info.GetCustomAttribute<ForeignKeyAttribute>() != null)
                {
                    level++;
                    type = info.PropertyType;
                }
                else
                    break;
            }
            return Math.Min(level, (byte)(report.ReportType.ConvertToInt().Value - 1));
        }
    }
}
