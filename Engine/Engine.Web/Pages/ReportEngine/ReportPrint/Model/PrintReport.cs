using Caspian.Engine;
using Caspian.Common;
using Caspian.Engine.Model;
using Caspian.Engine.Service;
using Caspian.Common.Extension;
using Microsoft.EntityFrameworkCore;

namespace ReportUiModels
{
    public class PrintReport
    {
        IServiceScope Scope;
        public PrintReport(IServiceScope scope)
        {
            Scope = scope;
        }

        public ReportParam ReportParamOfProperty(string infoName, IList<ReportParam> reportParams)
        {
            ReportParam param = null;
            foreach (var reportParam in reportParams)
            {
                var name = reportParam.TitleEn.Replace('.', '_');
                switch (reportParam.CompositionMethodType)
                {
                    case CompositionMethodType.Sum: name = "Sum_" + name; break;
                    case CompositionMethodType.Avg: name = "Avg_" + name; break;
                    case CompositionMethodType.Max: name = "Max_" + name; break;
                    case CompositionMethodType.Min: name = "Min_" + name; break;
                }
                if (infoName == name)
                {
                    param = reportParam;
                    break;
                }
            }
            return param;
        }

        public async Task<IDictionary<string, string>> GetFiledsProperty(int reportId, int dataLevel)
        {
            var reportGroupService = new ReportGroupService(Scope.ServiceProvider);
            var reportService = new ReportService(Scope.ServiceProvider);
            var paramService = new ReportParamService(Scope.ServiceProvider);
            var report = await reportService.SingleAsync(reportId);
            var reportGroup = await reportGroupService.SingleAsync(report.ReportGroupId);
            var mainType = new AssemblyInfo().GetReturnType(reportGroup);
            var reportParams = await paramService.GetAll().Include(t => t.DynamicParameter).
                Include(t => t.Rule).Where(t => t.ReportId == reportId && !t.IsKey).ToListAsync();
            var tempParams = reportParams.ToList();
            var selectReport = new SelectReport(mainType);
            //var type = selectReport.SimpleSelect(tempParams).Body.Type;
            var type = selectReport.GetEqualType(tempParams);
            type = new ReportPrintEngine(Scope.ServiceProvider).GetTypeOf(tempParams, type, mainType.Name);
            var maxDataLevel = tempParams.Max(t => t.DataLevel);
            var dic = new Dictionary<string, string>();
            if (maxDataLevel == dataLevel)
            {
                var reportParams1 = reportParams.Where(t => t.DataLevel == maxDataLevel).ToList();
                var dynamicParams = reportParams1.Where(t => t.RuleId != null || t.DynamicParameterId != null);
                if (dynamicParams.Any())
                {
                    var tempType = mainType;
                    var enTitle = dynamicParams.First().TitleEn;
                    if (enTitle.HasValue())
                        tempType = mainType.GetMyProperty(enTitle).PropertyType;
                    foreach(var item in dynamicParams)
                    {
                        if (item.RuleId.HasValue)
                            dic.Add("{list.DynamicParam" + item.RuleId + '}', item.Rule.Title);
                        else
                            dic.Add("{list.DynamicParam" + item.DynamicParameterId.Value + '}', item.DynamicParameter.Title);
                    }

                }
                foreach (var info in type.GetProperties())
                {
                    if (!info.PropertyType.IsGenericType || info.PropertyType.IsValueType)
                    {
                        string tempName = "", name = reportParams1.First().TitleEn;
                        if (maxDataLevel > 2)
                        {
                            tempName += name.Substring(0, name.IndexOf('.')) + '_';
                            name = name.Substring(name.IndexOf('.') + 1);
                        }
                        if (maxDataLevel > 1)
                            tempName += name.Substring(0, name.IndexOf('.')) + '_';
                        tempName += info.Name;
                        var param = ReportParamOfProperty(tempName, reportParams1.Where(t => t.RuleId == null && t.DynamicParameterId == null).ToList());
                        if (param != null)
                        {
                            name = param.TitleEn;
                            if (maxDataLevel > 1)
                                name = name.Substring(name.IndexOf('.') + 1);
                            if (maxDataLevel > 2)
                                name = name.Substring(name.IndexOf('.') + 1);
                            name = name.Replace('.', '_');
                            switch (param.CompositionMethodType)
                            {
                                case CompositionMethodType.Sum: name = "Sum_" + name; break;
                                case CompositionMethodType.Avg: name = "Avg_" + name; break;
                                case CompositionMethodType.Max: name = "Max_" + name; break;
                                case CompositionMethodType.Min: name = "Min_" + name; break;
                            }
                            var title = new ReportControlModel(mainType).GetFaTitle(param.TitleEn, param.RuleId, param.CompositionMethodType);
                            dic.Add("{list." + name + '}', title);
                        }
                    }
                }
            }
            if (dataLevel == maxDataLevel - 1)
            {
                var reportParams2 = reportParams.Where(t => t.DataLevel == maxDataLevel - 1).ToList();
                var info = type.GetProperties().Single(t => t.PropertyType.IsGenericType && !t.PropertyType.IsValueType);
                foreach (var info1 in info.PropertyType.GetGenericArguments()[0].GetProperties())
                {
                    if (!info1.PropertyType.IsGenericType || info1.PropertyType.IsValueType)
                    {
                        string tempName = "", name = reportParams2.First().TitleEn;
                        if (maxDataLevel == 3)
                            tempName += name.Substring(0, name.IndexOf('.')) + '_';
                        tempName += info1.Name;
                        var param1 = ReportParamOfProperty(tempName, reportParams2);
                        if (param1 != null)
                        {
                            var title = new ReportControlModel(mainType).GetFaTitle(param1.TitleEn, param1.RuleId, param1.CompositionMethodType);
                            dic.Add("{list." + info.Name + '.' + info1.Name + '}', title);
                        }
                    }
                }
            }
            if (dataLevel == maxDataLevel - 2)
            {
                var reportParams3 = reportParams.Where(t => t.DataLevel == maxDataLevel - 2).ToList();
                var info = type.GetProperties().Single(t => t.PropertyType.IsGenericType && !t.PropertyType.IsValueType);
                var name = info.Name.Replace('_', '.');
                var info1 = info.PropertyType.GetGenericArguments()[0].GetProperties()
                    .Single(t => t.PropertyType.IsGenericType && !t.PropertyType.IsValueType);
                foreach (var info2 in info1.PropertyType.GetGenericArguments()[0].GetProperties())
                {
                    var param2 = ReportParamOfProperty(info2.Name, reportParams3);
                    if (param2 != null)
                    {
                        var title = new ReportControlModel(mainType).GetFaTitle(param2.TitleEn, param2.RuleId, param2.CompositionMethodType);
                        dic.Add("{list." + info.Name + '.' + info1.Name + '.' + info2.Name + '}', title);
                    }
                }
            }
            return dic;
        }

    }
}
