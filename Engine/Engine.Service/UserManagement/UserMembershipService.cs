using Caspian.Engine.Model;
using Caspian.Common.Service;
using Microsoft.Extensions.DependencyInjection;

namespace Caspian.Engine.Service
{
    public class UserMembershipService : SimpleService<UserMembership>
    {
        public UserMembershipService(IServiceProvider provider) :
            base(provider)
        {

        }
    }
}
