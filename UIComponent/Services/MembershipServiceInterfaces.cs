namespace Caspian.UI
{
    public interface IMembershipService<TMaster, TAccess, TOther>: IUIService<TAccess> where TAccess : class where TOther : class
    {
        ISearchService<TOther> OtherService{ get; }
    }
}
