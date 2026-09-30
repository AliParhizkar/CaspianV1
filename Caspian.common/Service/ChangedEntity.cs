namespace Caspian.Common
{
    public class ChangedEntity<TEntity>
    {
        public TEntity Entity { get; set; }

        public ChangeType ChangeType { get; set; }
    }

    public enum ChangeType
    {
        Added = 1,

        Updated,

        Deleted,
    }
}
