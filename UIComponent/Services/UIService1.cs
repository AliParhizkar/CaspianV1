using Caspian.Common;
using System.Reflection;
using Caspian.Common.Service;
using System.Linq.Expressions;
using Caspian.Common.Extension;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace Caspian.UI
{
    internal class UIService<TMaster, TDetail>: UIService<TMaster>, IUIService<TMaster, TDetail> 
        where TMaster : class where TDetail : class
    {
        public UIService(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            DetailService = new BatchService<TMaster, TDetail>(UpsertData);
        }

        public IBatchService<TDetail> DetailService { get; set; }

        internal IInternalBatchService<TDetail> BatchService { get { return DetailService as IInternalBatchService<TDetail>; }}

        /// <summary>
        /// This method reload data for update
        /// </summary>
        /// <param name="masterId">The id of "TMaster"</param>
        public async Task ReloadForUpdate(int masterId)
        {
            typeof(TMaster).GetPrimaryKey().SetValue(UpsertData, masterId);
            await BatchService.ReloadForUpdate(masterId);
            await (this as IInternalUIService<TMaster>).FetchAsync();
        }

        protected override IBaseService<TMaster> CreateService(IServiceScope scope)
        {
            return scope.GetService<IMasterDetailsService<TMaster, TDetail>>();
        }

        protected override TMaster InitializeBeforeUpsert(IBaseService<TMaster> service)
        {
            var masterDetailService = service as IMasterDetailsService<TMaster, TDetail>;
            masterDetailService.DetailService.SetChangedEntities(DetailService.ChangedEntities);
            return base.InitializeBeforeUpsert(service);
        }

        protected override async Task InitializeAfterUpsert(TMaster tempEntity, UpsertMode upsertMode)
        {
            BatchService.InitializeAfterUpsert();
            await base.InitializeAfterUpsert(tempEntity, upsertMode);
        }

        protected override async Task InitializeOnUpsert(TMaster master)
        {
            var value = typeof(TMaster).GetPrimaryKey().GetValue(master);
            BatchService.MasterId = Convert.ToInt32(value);
            BatchService.ClearChangedEntities();
            await base.InitializeOnUpsert(master);
        }

        protected virtual Task SetChangedEntities()
        {
            var masterDetailService = Form?.ValidationValidator?.Validator as IMasterDetailsService<TMaster, TDetail>;
            if (masterDetailService != null)
            {
                masterDetailService.DetailService.SetChangedEntities(DetailService.ChangedEntities);
                masterDetailService.DetailService.SetDetails(DetailService.Details);
            }
            return Task.CompletedTask;
        }

        protected override async Task InitializeValidatorService(IBaseService<TMaster> service)
        {
            var masterDetailService = service as IMasterDetailsService<TMaster, TDetail>;
            masterDetailService.DetailService.SetChangedEntities(DetailService.ChangedEntities);
            masterDetailService.DetailService.SetDetails(DetailService?.Details);
            await base.InitializeValidatorService(service);
        }
    }
}