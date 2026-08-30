using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caspian.UI
{
    public class UIServiceFactory
    {
        public static IMembershipService<TMaster, TAccess, TOther> CreateAccessService<TMaster, TAccess, TOther>(IServiceProvider provider)
            where  TAccess : class where TOther : class
        {
            return new MembershipService<TMaster, TAccess, TOther>(provider);
        }
    }
}
