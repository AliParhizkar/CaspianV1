namespace Caspian.Common
{
    public class ChangedEntity<TEntity>
    {
        public TEntity Entity { get; set; }

        public ChangeStatus ChangeStatus { get; set; }
    }

    public enum ChangeStatus
    {
        Added = 1,

        Updated,

        Deleted,
    }
}
