using Caspian.Common;
using Caspian.Common.Extension;
using Caspian.Common.Service;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace Caspian.UI
{
    public partial class TypeWindow<TEntity>: ComponentBase where TEntity : class 
    {
        Type serviceType;
        WindowStatus status;
        TEntity newEntity;
        CaspianForm<TEntity> form;
        IList<TEntity> source;
        IDictionary<string, object> windowProperties;

        async Task UpdateEntity(TEntity entity)
        {
            entity.CopyEntity(newEntity);
            if (OnSubmit.HasDelegate)
                await OnSubmit.InvokeAsync(entity);
        }

        [Parameter]
        public EventCallback<TEntity> OnSubmit {  get; set; }
        
        [Parameter]
        public IDetailBatchService<TEntity> Service { get; set; }

        [Parameter]
        public int ColumnsCount { get; set; }

        [Parameter]
        public RenderFragment<TEntity> ChildContent { get; set; }

        [Parameter]
        public string Title { get; set; }

        [Parameter]
        public string Style { get; set; }

        [Parameter]
        public EventCallback<TEntity> OnOpen { get; set; }

        public async Task OpenAsync(TEntity entity, IList<TEntity> source)
        {
            this.source = source;
            status = WindowStatus.Open;
            using var service = ScopeFactory.CreateScope().GetService<BaseService<TEntity>>();
            serviceType = service.GetType();
            if (OnOpen.HasDelegate)
                await OnOpen.InvokeAsync(entity);
            newEntity = entity.CreateNewEntity(); ;
        }

        public void Close()
        {
            status = WindowStatus.Close;
            Service.DetailForm = null;
        }

        protected override void OnParametersSet()
        {
            windowProperties = new Dictionary<string, object>();
            if (Title.HasValue())
                windowProperties.Add("Title", Title);
            if (Style.HasValue())
                windowProperties.Add("Style", Style);
            base.OnParametersSet();
        }

        protected override void OnInitialized()
        {
            if (Service != null)
            {
                Service.TypeWindow = this;
                Service.DetailTypwWindowInitialize();
            }
            base.OnInitialized();
        }

        protected override void OnAfterRender(bool firstRender)
        {
            if (form != Service.DetailForm)
            {
                Service.DetailForm = form;
                Service.DetailFormInitialize();
            }
            base.OnAfterRender(firstRender);
        }
    }
}
