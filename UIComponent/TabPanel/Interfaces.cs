
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
}
