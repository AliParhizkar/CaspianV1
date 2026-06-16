
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
    /// Use this class to send Master-Service data (Master Id & Master Type) to Details-Service in tab panel component, 
    /// To find foreign key property (by Master type) and initialize it (by Master Id)
    /// </summary>
    internal class MasterDetailsCrudServiceData
    {
        public int MasterId { get; set; }

        public Type MasterType { get; set; }
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
