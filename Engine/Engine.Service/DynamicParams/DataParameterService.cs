using Caspian.Common;
using Caspian.Common.Service;
using Microsoft.Extensions.DependencyInjection;

namespace Caspian.Engine.Service
{
    public class DataParameterService : SimpleService<DataParameter>, ISimpleService<DataParameter>
    {
        public DataParameterService(IServiceScope scope)
            :base(scope)
        {
            RuleFor(t => t.PropertyName).Required(t => t.ParameterType == DataParameterType.EntityProperties)
                .UniqAsync(t => t.ResultParameterId, "این فیلد قبلا در فرم مورد استفاده قرار گرفته است");
            RuleFor(t => t.DynamicParameterId).Required(t => t.ParameterType == DataParameterType.DynamicParameters)
                .UniqAsync(t => t.ResultParameterId, "این پارامتر قبلا مورد استفاده قرار گرفته است");
            RuleFor(t => t.RuleId).Required(t => t.ParameterType == DataParameterType.FormRule)
                .UniqAsync(t => t.ResultParameterId, "این قانون قبلا مورد استفاده قرار گرفته است");
        }
    }
}
