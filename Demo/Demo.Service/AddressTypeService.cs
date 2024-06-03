using System;
using Demo.Model;
using Caspian.Common;
using Caspian.Common.Service;

namespace Demo.Service
{
    public class AddressTypeService : BaseService<AddressType>, IBaseService<AddressType>
    {
        public AddressTypeService(IServiceProvider provider)
            : base(provider)
        {
            RuleFor(t => t.Title).Required().UniqAsync("A type with this title is defined in the system");
        }
    }
}
