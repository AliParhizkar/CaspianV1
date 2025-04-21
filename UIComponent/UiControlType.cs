
using Microsoft.AspNetCore.Components.Forms;

namespace Caspian.UI
{
    public class CancelableEvent<TEntity>
    {

        public CancelableEvent(TEntity entity) 
        {
            Entity = entity;
        }
        public bool Cancel { get; set; }

        public TEntity Entity { get; set; }
    }
}
