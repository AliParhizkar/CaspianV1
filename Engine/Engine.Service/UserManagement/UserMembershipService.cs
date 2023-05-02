using Caspian.Engine.Model;
using Caspian.Common.Service;
using Microsoft.Extensions.DependencyInjection;

namespace Caspian.Engine.Service
{
    public class UserMembershipService : BaseService<UserMembership>
    {
        public UserMembershipService(IServiceProvider provider) :
            base(provider)
        {

        }
    }
}
