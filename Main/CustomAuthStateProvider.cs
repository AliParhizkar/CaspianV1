using Caspian.Common.Extension;
using Caspian.Engine.Model;
using Caspian.Engine.Service;
using Caspian.Engine;
using Microsoft.AspNetCore.Components.Authorization;
using System.Linq.Dynamic.Core;
using System.Security.Claims;
using System.Collections;
using Microsoft.EntityFrameworkCore;
using Caspian.UI;

namespace Main
{
    //public class CustomAuthStateProvider: AuthenticationStateProvider
    //{
    //    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    //    {
    //        var identity = new ClaimsIdentity(new[]
    //        {
    //        new Claim(ClaimTypes.Name, "mrfibuli"),
    //    }, "Fake authentication type");

    //        var user = new ClaimsPrincipal(identity);

    //        return Task.FromResult(new AuthenticationState(user));
    //    }
    //}




    public class ReportPrintEngine
    {
        IServiceProvider ServiceProvider;

        public ReportPrintEngine(IServiceProvider provider)
        {
            ServiceProvider = provider;
        }


        public IList GetData(int reportId, IQueryable data)
        {
            var service = ServiceProvider.GetService<ReportParamService>();
            var reportParams = service.GetAll().Include(t => t.DynamicParameter).Include(t => t.Rule)
                .Include(t => t.ReportGroupParameter).Where(t => t.ReportId == reportId).ToList();
            var report = new SelectReport(data.ElementType);
            Type type = null;
            if (reportParams.Any(t => t.CompositionMethodType.HasValue))
            {
                var lambda = report.SelectForGroupBy(reportParams);
                data = data.GroupBy(report.GroupBy(reportParams)).Select(lambda);
            }
            else
            {
                var lambda = report.SimpleSelect(reportParams, out type);
                data = data.Select(lambda);
            }
            var list = data.ToIList();
            return report.GetStumlData(list, reportParams, type, reportParams.Max(t => t.DataLevel));
            //return report.GetValues(data, reportParams);
        }

        public Type GetTypeOf(IList<ReportParam> reportParams, Type dynamicType, string mainTypeName)
        {
            var dataParam = reportParams.Where(t => t.DataLevel == 1 && !t.ReportGroupParameter.IsKey);
            var propertiesList = new List<DynamicProperty>();
            foreach (var param in dataParam)
            {
                string name = null;
                if (param.RuleId.HasValue || param.DynamicParameterId.HasValue)
                    name = "DynamicParam" + (param.RuleId ?? param.DynamicParameterId.Value);
                else
                    name = param.ReportGroupParameter.TitleEn.Replace('.', '_');
                switch (param.CompositionMethodType)
                {
                    case CompositionMethodType.Sum: name = "Sum_" + name; break;
                    case CompositionMethodType.Avg: name = "Avg_" + name; break;
                    case CompositionMethodType.Max: name = "Max_" + name; break;
                    case CompositionMethodType.Min: name = "Min_" + name; break;
                }
                propertiesList.Add(new DynamicProperty(name, dynamicType.GetProperty(param.ReportGroupParameter.TitleEn).PropertyType));
            }
            var type = DynamicClassFactory.CreateType(propertiesList, false);
            dataParam = reportParams.Where(t => t.DataLevel == 2 && !t.ReportGroupParameter.IsKey);
            if (dataParam.Any())
            {
                propertiesList.Clear();
                foreach (var param in dataParam)
                {
                    var tempType = dynamicType.GetProperty(param.ReportGroupParameter.TitleEn).PropertyType;
                    //name = GetGroupingFiledName(param.TitleEn, 2);
                    propertiesList.Add(new DynamicProperty(param.ReportGroupParameter.TitleEn.Replace('.', '_'), tempType));
                }
                var listType = typeof(List<>);
                listType = listType.MakeGenericType(type);
                propertiesList.Add(new DynamicProperty(mainTypeName + 's', listType));
                type = DynamicClassFactory.CreateType(propertiesList, false);
                dataParam = reportParams.Where(t => t.DataLevel == 3 && !t.ReportGroupParameter.IsKey);
                if (dataParam.Any())
                {
                    propertiesList.Clear();
                    foreach (var param in dataParam)
                    {
                        var tempType = dynamicType.GetProperty(param.ReportGroupParameter.TitleEn).PropertyType;
                        //name = GetGroupingFiledName(param.TitleEn, 3);
                        propertiesList.Add(new DynamicProperty(param.ReportGroupParameter.TitleEn.Replace('.', '_'), tempType));
                    }
                    listType = typeof(List<>);
                    listType = listType.MakeGenericType(type);
                    var name2 = GetPropertyListName(dataParam.First().ReportGroupParameter.TitleEn, 3);
                    propertiesList.Add(new DynamicProperty(name2, listType));
                    type = DynamicClassFactory.CreateType(propertiesList, false);
                }
            }
            return type;
        }

        private string GetPropertyListName(string name, int dataLevel)
        {
            var str = name;
            if (dataLevel > 1)
            {
                var index = str.LastIndexOf('.');
                str = str.Substring(0, index);
            }
            if (dataLevel > 2)
            {
                var index = str.LastIndexOf('.');
                str = str.Substring(0, index);
            }
            return str + 's';
        }
    }
}
