using Caspian.Common;
using System.Reflection;
using Caspian.Engine.Model;
using Caspian.Common.Service;
using FluentValidation.Results;
using Caspian.Common.Extension;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Extensions.DependencyInjection;

namespace Caspian.Engine.Service
{
    public class ReportParamService : BaseService<ReportParam>
    {
        public ReportParamService(IServiceProvider provider)
            :base(provider)
        {
            RuleForRemove().CustomAsync(async t => 
            {
                
                var param = await provider.GetService<ReportParamService>().GetAll().Include(t => t.Report)
                    .SingleAsync(t.Id);
                return param.Report.PrintFileName.HasValue();
            }, "After creating the report, it is not possible to delete the report parameters");
        }

        public IQueryable<ReportParam> GetAll(int reportId)
        {
            return GetAll().Where(t => t.ReportId == reportId);
        }

        public async override Task<ValidationResult> ValidateRemoveAsync(ReportParam entity)
        {

            return await base.ValidateRemoveAsync(entity);
        }

        public IQueryable<ReportParam> GetAll(int reportId, int dataLevel)
        {
            return GetAll().Where(t => t.ReportId == reportId && t.DataLevel == dataLevel);
        }

        async public Task AddAllForGroupBy(IEnumerable<ReportParam> list)
        {
            var reportId = list.First().ReportId;
            var oldParams = await GetAll(reportId).ToListAsync();
            if (!oldParams.Any(t => !t.CompositionMethodType.HasValue))
                throw new Exception("Please select the main parameters first and then select the management parameters", null);
            
            foreach (var param in list)
            {
                if (!oldParams.Any(t => t.TitleEn == param.TitleEn))
                {
                    var service = GetService<ReportParamService>();
                    await service.AddAsync(param);
                    await service.SaveChangesAsync();
                }
            }
        }

        public async Task AddAll(IEnumerable<ReportParam> list)
        {
            var reportId = list.First().ReportId;
            var oldParams = await GetAll(reportId).ToListAsync();
            list = list.Where(p => !oldParams.Any(t => t.TitleEn != null && t.TitleEn == p.TitleEn || t.RuleId.HasValue && t.RuleId == p.RuleId));
            var a = new ReportParamService(ServiceProvider);
            await a.AddRangeAsync(list);
            await a.SaveChangesAsync();
        }

        async public Task IncOrder(int reportParamId)
        {
            var curent = await SingleOrDefaultAsync(reportParamId);
            var maxOrder = GetAll(curent.ReportId).Where(t => t.Order_ < curent.Order_).Max(t => t.Order_);
            var item = GetAll().SingleOrDefault(t => t.ReportId == curent.ReportId && t.Order_ == maxOrder);
            var temp = item.Order_;
            item.Order_ = curent.Order_;
            curent.Order_ = temp;
            await SaveChangesAsync();
        }

        async public Task DecOrder(int reportParamId)
        {
            var curent = await SingleOrDefaultAsync(reportParamId);
            var minOrder = GetAll(curent.ReportId).Where(t => t.Order_ > curent.Order_).Min(t => t.Order_);
            var item = GetAll().SingleOrDefault(t => t.ReportId == curent.ReportId && t.Order_ == minOrder);
            var temp = item.Order_;
            item.Order_ = curent.Order_;
            curent.Order_ = temp;
            await SaveChangesAsync();
        }

        async public Task<ReportParam> IncDataLevel(int id)
        {
            var temp = await SingleAsync(id);
            if (temp.DataLevel.GetValueOrDefault(1) > 2)
                throw new CaspianException("It is not possible to increase the level");
            var report = await new ReportService(ServiceProvider).GetAll().Where(t => t.Id == temp.ReportId).Include(t => t.ReportGroup).SingleAsync();
            if (report.PrintFileName.HasValue())
                throw new CaspianException("After creating the report, it is not possible to increase the field level");
            var type = new AssemblyInfo().GetReturnType(report.ReportGroup);
            var maxDataLevel = MaxDataLevel(temp.TitleEn, type, temp.CompositionMethodType);
            if (temp.DataLevel + 1 > maxDataLevel)
                throw new CaspianException("It is not possible to increase the level for this field", null);
            temp.DataLevel++;
            return temp;
        }

        private int MaxDataLevel(string enTitle, Type type, CompositionMethodType? methodType)
        {
            if (methodType.HasValue)
                return 1;
            var level = 1;
            foreach (var str in enTitle.Split('.'))
            {
                var info = type.GetProperty(str);
                type = info.PropertyType;
                var foreignKeyAttribute = info.GetCustomAttribute<ForeignKeyAttribute>();
                if (foreignKeyAttribute == null)
                    break;
                level++;
            }
            return level;
        }

        async public Task<ReportParam> DecDataLevel(int id)
        {
            var temp = await GetAll().Include(t => t.Report).SingleAsync(id);
            if (temp.DataLevel.GetValueOrDefault(1) <= 1)
                throw new CaspianException("It is not possible to reduce the level", null);
            if (temp.Report.PrintFileName.HasValue())
                throw new CaspianException("After creating the report, it is not possible to reduce the field level", null);
            temp.DataLevel--;
            return temp;
        }

        public async void DeleteDataKey(int reportId, int dataLevel)
        {
            if (dataLevel > 1 && !GetAll().Any(t => t.ReportId == reportId && t.DataLevel == dataLevel && !t.IsKey))
            {
                var temp = GetAll().SingleOrDefault(t => t.ReportId == reportId && t.DataLevel == dataLevel && t.IsKey);
                if (temp != null)
                {
                    await base.RemoveAsync(new ReportParam()
                    {
                        Id = temp.Id
                    });
                }
            }
        }

        async public Task AddDataKey(int reportId, byte dataLevel)
        {
            if (dataLevel > 1 && !GetAll().Any(t => t.ReportId == reportId && t.DataLevel == dataLevel && t.IsKey))
            {
                var param = GetAll().First(t => t.ReportId == reportId && t.DataLevel == dataLevel);
                var type = new AssemblyInfo().GetReturnType(param.Report.ReportGroup);
                var enTitle = "";
                var array = param.TitleEn.Split('.');
                var peropertyInfo = type.GetProperty(array[0]);
                type = peropertyInfo.PropertyType;
                if (dataLevel == 3)
                {
                    enTitle = array[0];
                    peropertyInfo = type.GetProperty(array[1]);
                }
                if (enTitle.HasValue())
                    enTitle += '.';
                var foreignKeyAttribute = peropertyInfo.GetCustomAttribute<ForeignKeyAttribute>();
                if (foreignKeyAttribute == null)
                    throw new Exception("It is not possible to increase the level for this field", null);
                enTitle += foreignKeyAttribute.Name;
                var keyParam = new ReportParam()
                {
                    TitleEn = enTitle,
                    DataLevel = dataLevel,
                    IsKey = true,
                    ReportId = param.ReportId
                };
                await base.AddAsync(keyParam);
            }
        }
    }
}
