using System;
using System.Threading.Tasks;

namespace Caspian.UI
{
    public interface IEnableLoadData
    {
        void EnableLoading();
    }

    public interface ICascading
    {
        void CascadTo(Type masterType, object value);
        Task SetValue(object value);
    }

    public interface IWindow: IDisposable
    {
        Task Close();
        
    }

    public class CascadeService 
    {
        public ICascading Cascade { get; set; }
    }

    //public interface IGridButtonCommand<TEntity>
    //{
    //    Task OpenForm(TEntity entity);
    //    Task Delete(TEntity entity);
    //}

    public class EnableLoadContiner
    {
        public IEnableLoadData Control { get; set; }
    }
}
