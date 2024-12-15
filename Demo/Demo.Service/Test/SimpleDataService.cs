using System;
using Demo.Model;
using Caspian.Common;
using Caspian.Common.Service;

namespace Demo.Service
{
    public class SimpleDataService : BaseService<SimpleData>, IBaseService<SimpleData>
    {
        public SimpleDataService(IServiceProvider provider)
            : base(provider)
        {
            RuleFor(t => t.Title).Required().UniqueAsync(t => t.DataType, "این عنوان تکراری است");
        }
    }
}
