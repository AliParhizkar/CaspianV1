using Caspian.Common;
using System.Reflection;
using System.Collections;
using Caspian.Engine.Model;
using Caspian.Common.Extension;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Caspian.Engine.Service
{
    public class ReportPrintEngine
    {
        IServiceProvider ServiceProvider;

        public ReportPrintEngine(IServiceProvider provider)
        {
            ServiceProvider = provider;
        }


        public async Task<IList> GetData(int reportId, IQueryable data)
        {
            var service = ServiceProvider.GetService<ReportParamService>();
            var reportParams = await service.GetAll().Include(t => t.Rule)
                .Include(t => t.ReportGroupParameter).Where(t => t.ReportId == reportId).ToListAsync();
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
                var maxLevel = reportParams.Max(t => t.DataLevel);
                if (maxLevel == 1)
                    return report.GetValues(data, reportParams, type);
                var list = await data.ToDynamicListAsync();
                var multiLevelType = GetMultiLevelType(type, reportParams, maxLevel);
                var multiLevelsData =  GetDataOfLevel(list, reportParams, type, maxLevel);
                var items = new ArrayList();
                foreach ( var item in multiLevelsData )
                {
                    var converteddata = ConvertDataToMultiLevelObject(item, multiLevelType);
                    items.Add(converteddata);
                }
                return items;
            }

            throw new NotImplementedException();
            //
        }

        object ConvertDataToMultiLevelObject(ReportLevelData levelData, Type multiLevelType)
        {
            var data = Activator.CreateInstance(multiLevelType);
            CopyObject(data, levelData.Master);
            if (levelData.Values != null)
                CreateDetails(data, levelData.Values);
            if (levelData.Details != null)
            {
                var detailInfo = multiLevelType.GetProperty("__Details");
                var detailType = detailInfo.PropertyType.GetGenericArguments()[0];
                var array = Array.CreateInstance(detailType, levelData.Details.Count);
                var index = 0;
                foreach ( var item in levelData.Details)
                {
                    var detail = Activator.CreateInstance(detailType);
                    CopyObject(detail, item.Master);
                    CreateDetails(detail, item.Values);
                    array.SetValue(detail, index);
                    index++;
                }
                detailInfo.SetValue(data, array);
            }
            return data;
        }

        void CreateDetails(object master, IList values)
        {
            var masterType = master.GetType();
            var detailsInfo = masterType.GetProperty("__Details");
            var genericType = detailsInfo.PropertyType.GetGenericArguments()[0];
            var array = Array.CreateInstance(genericType, values.Count);
            var index = 0;
            foreach (var value in values)
            {
                var newValue = Activator.CreateInstance(genericType);
                CopyObject(newValue, value);
                array.SetValue(newValue, index);
                index++;
            }
            var details = Activator.CreateInstance(typeof(List<>).MakeGenericType(genericType), new object[] { array });
            detailsInfo.SetValue(master, details);
        }

        void CopyObject(object data, object levelData)
        {
            var levelDataType = levelData.GetType();
            foreach(var info in data.GetType().GetProperties().Where(t => t.IsCollectible))
            {
                if (info.Name != "__Details")
                {
                    var levelDataInfo = levelDataType.GetProperty(info.Name);
                    var value = levelDataInfo.GetValue(levelData);
                    if (levelDataInfo.PropertyType.IsEnumType())
                        value = (value as Enum).EnumText();
                    info.SetValue(data, value);
                }
            }
        }

        IList<ReportLevelData> GetDataOfLevel(IList source, IList<ReportParam> reportParams, Type type, int level)
        {
            var keyparam = reportParams.Single(t => t.DataLevel == level && t.ReportGroupParameter.IsKey);
            var keyInfo = type.GetProperty(keyparam.ReportGroupParameter.TitleEn.Replace(".", ""));
            ReportParam keyparam1 = null;
            PropertyInfo keyInfo1 = null;
            if (level == 3)
            {
                keyparam1 = reportParams.SingleOrDefault(t => t.DataLevel == 2 && t.ReportGroupParameter.IsKey);
                keyInfo1 = type.GetProperty(keyparam1.ReportGroupParameter.TitleEn.Replace(".", ""));
            }
            var values = new List<ReportLevelData>();
            foreach (var item in source)
            {
                var keyvalue = (int?)keyInfo.GetValue(item);
                var old = values.SingleOrDefault(t => t.Key == keyvalue);
                if (old == null)
                {
                    var data = new ReportLevelData();
                    data.Key = keyvalue;
                    data.Master = item;
                    if (level == 2)
                        data.Values = new ArrayList() { item };
                    else
                    {
                        var keyvalue1 = (int?)keyInfo1.GetValue(item);
                        data.Details = new List<ReportLevelData>()
                        {
                            new ReportLevelData()
                            {
                                Key = keyvalue1,
                                Master = item,
                                Values = new ArrayList(){item}
                            }
                        };
                    }
                    values.Add(data);
                }
                else
                {
                    if (level == 3)
                    {
                        var keyvalue1 = (int?)keyInfo1.GetValue(item);
                        var old1 = old.Details.SingleOrDefault(t => t.Key == keyvalue1);
                        if (old1 == null)
                        {
                            old.Details.Add(new ReportLevelData()
                            {
                                Key = keyvalue1,
                                Master = item,
                                Values = new ArrayList() { item }
                            });
                        }
                        else
                            old1.Values.Add(item);
                    }
                    else
                        old.Values.Add(item);
                }
            }
            return values;
        }

        public Type GetMultiLevelType(Type mainType, IList<ReportParam> reportParams, int level)
        {
            var list = new List<DynamicProperty>();
            foreach (var param in reportParams.Where(t => t.DataLevel == level))
            {
                var name = param.ReportGroupParameter.TitleEn.Replace(".", "");
                var type = mainType.GetProperty(name).PropertyType;
                if (type.GetUnderlyingType().IsEnum)
                    type = typeof(string);
                list.Add(new DynamicProperty(name, type));
            }
            if (level > 1)
            {
                var type = GetMultiLevelType(mainType, reportParams, level - 1);
                type = typeof(IList<>).MakeGenericType(type);
                list.Add(new DynamicProperty("__Details", type));
            }
            return DynamicClassFactory.CreateType(list);
        }
    }
}
