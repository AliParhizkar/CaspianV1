namespace Caspian.UI
{
    public class SimplePageService<TEntity> where TEntity : class
    {
        public DataGrid<TEntity> Grid { get; set; }

        public IWindow Window { get; set; } 

        public CaspianForm<TEntity> Form { get; set; }
    }
}
