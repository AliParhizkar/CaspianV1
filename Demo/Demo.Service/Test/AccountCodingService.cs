using System;
using Demo.Model;
using Caspian.Common.Service;

namespace Demo.Service
{
    public class AccountCodingService : BaseService<AccountCoding>
    {
        public AccountCodingService(IServiceProvider provider)
            : base(provider)
        {

        }
    }
}
