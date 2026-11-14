using Caspian.Common;
using System.Reflection;
using Caspian.Common.Service;
using System.Linq.Expressions;
using Caspian.Common.Extension;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace Caspian.UI
{
    internal class UIService<TMaster, TDetail, TDetail1> : UIService<TMaster, TDetail>, IUIService<TMaster, TDetail, TDetail1>
        where TMaster : class where TDetail : class where TDetail1 : class
    {
        public UIService(IServiceProvider provider):
            base(provider)
        {
            Detail2Service = new BatchService<TMaster, TDetail1>(provider, UpsertData);
        }

        public IBatchService<TDetail1> Detail2Service { get; set; }

        internal IInternalBatchService<TDetail1> Batch2Service { get { return Detail2Service as IInternalBatchService<TDetail1>; } }

        /// <summary>
        /// This method reload data for update
        /// </summary>
        /// <param name="masterId">The id of "TMaster"</param>
        public new async Task ReloadForUpdate(int masterId)
        {
            typeof(TMaster).GetPrimaryKey().SetValue(UpsertData, masterId);
            await BatchService.ReloadForUpdate(masterId);
            await Batch2Service.ReloadForUpdate(masterId);
            await (this as IInternalUIService<TMaster>).FetchAsync();
        }

        protected override IBaseService<TMaster> CreateService(IServiceScope scope)
        {
            return scope.GetService<IMasterDetailsService<TMaster, TDetail, TDetail1>>();
        }

        protected override TMaster InitializeBeforeUpsert(IBaseService<TMaster> service)
        {
            var masterDetailService = service as MasterDetailsService<TMaster, TDetail, TDetail1>;
            masterDetailService.Detail2Service.SetChangedEntities(Detail2Service.ChangedEntities);
            return base.InitializeBeforeUpsert(service);
        }

        protected override async Task InitializeAfterUpsert(TMaster tempEntity, UpsertMode upsertMode)
        {
            Batch2Service.InitializeAfterUpsert();
            await base.InitializeAfterUpsert(tempEntity, upsertMode);
        }

        protected override async Task InitializeOnUpsert(TMaster master)
        {
            var value = typeof(TMaster).GetPrimaryKey().GetValue(master);
            Batch2Service.MasterId = Convert.ToInt32(value);
            Batch2Service.ClearChangedEntities();
            await base.InitializeOnUpsert(master);
        }


        /// <summary>
        /// This Method Initialize Validator Service Data "ChangedEntities"(All change on entity) and "Details" (All Detail entity)
        /// </summary>
        /// <param name="service">Validator Service(MasterDetailsService<TMaster, TDetail>) that used for validation</param>
        /// <returns></returns>
        protected override void InitializeValidatorService(IBaseService<TMaster> service)
        {
            var masterDetailService = service as MasterDetailsService<TMaster, TDetail, TDetail1>;
            masterDetailService.Detail2Service.SetChangedEntities(Detail2Service.ChangedEntities);
            masterDetailService.Detail2Service.SetDetails(Detail2Service.Details);
            base.InitializeValidatorService(service);
        }
    }
}
