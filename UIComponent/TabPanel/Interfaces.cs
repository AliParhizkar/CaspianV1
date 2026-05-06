
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
    internal class MasterServiceData
    {
        public int MasterId { get; set; }

        public Type MasterType { get; set; }

        public Type DetailType { get; set; }
    }
}
