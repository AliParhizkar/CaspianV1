using Caspian.Common;
using Caspian.Common.Extension;
using Caspian.Common.Service;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;

namespace Caspian.UI
{
    public partial class TypeWindow<TEntity>: ComponentBase where TEntity : class 
    {
        Type serviceType;
        WindowStatus status;
        TEntity entity;
        TEntity newEntity;
        CaspianForm<TEntity> form;
        IList<TEntity> source;
        IDictionary<string, object> windowProperties;

        void UpdateEntity(TEntity entity)
        {
            entity.CopyEntity(newEntity);
        }
        
        [Parameter]
        public IDetailBatchService<TEntity> Service { get; set; }

        [Parameter]
        public RenderFragment<TEntity> Content { get; set; }

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
            StateHasChanged();
            this.entity = entity;
            newEntity = Activator.CreateInstance<TEntity>();
            newEntity.CopyEntity(entity);
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
