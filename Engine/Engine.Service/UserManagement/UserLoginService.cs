using Caspian.Engine.Model;
using Caspian.Common.Service;

namespace Caspian.Engine.Service
{
    public class UserLoginService : BaseService<UserLogin>
    {
        public UserLoginService(IServiceProvider provider) :
            base(provider)
        {

        }
    }
}
