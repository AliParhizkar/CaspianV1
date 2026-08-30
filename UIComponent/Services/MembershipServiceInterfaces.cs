using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Caspian.UI
{
    public interface IMembershipService<TMaster, TAccess, TOther>: IUIService<TAccess> where TAccess : class where TOther : class
    {
        IQueryable<TAccess> GetFilteredAccess(IServiceScope scope);

        IQueryable<TOther> GetFilteredOthers(IServiceScope scope);

        int MasterId { get; set; }

        DataView<TOther> OtherDataView { get; }

        ISearchService<TOther> OtherService { get; }

        Task RemoveAsync();
    }
}
