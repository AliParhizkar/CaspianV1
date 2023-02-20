using System;

namespace Caspian.UI
{
    public class GridService<TEntity>
    {
        public GridService() 
        {
            Search = Activator.CreateInstance<TEntity>();
            UpserData = Activator.CreateInstance<TEntity>();
        }
        public TEntity Search { get; set; }

        public TEntity UpserData { get; set; }
    }
}
