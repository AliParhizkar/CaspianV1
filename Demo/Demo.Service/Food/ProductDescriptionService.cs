using System;
using Demo.Model;
using Caspian.Common;
using Caspian.Common.Service;

namespace Demo.Service
{
    public class ProductDescriptionService : BaseService<ProductDescription>, IBaseService<ProductDescription>
    {
        public ProductDescriptionService(IServiceProvider provider)
            : base(provider)
        {
            RuleFor(t => t.Description).Required().UniqueAsync("This product has description width this context");
        }
    }
}
