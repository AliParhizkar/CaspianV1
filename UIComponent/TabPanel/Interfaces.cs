
namespace Caspian.UI
{
    public interface IEntityTabPanel
    {
        void ChangeState();
    }

    internal interface IEntityTabPanelItem
    {
        Type GetEntityType();

        bool IsChildrenTabPanelItem();
    }

    /// <summary>
    /// Use this class to send Master-Service data to Details-Service in tab panel component
    /// </summary>
    internal class MasterDetailsCrudServiceData
    {
        public int MasterId { get; set; }

        public Type MasterType { get; set; }

        public Type DetailType { get; set; }
    }

    /// <summary>
    /// Use this class to keep Master_Other data
    /// </summary>
    internal class MasterOtherCrudService
    {
        public int Id { get; set; }

        public Type OtherType { get; set; }
    }
}
