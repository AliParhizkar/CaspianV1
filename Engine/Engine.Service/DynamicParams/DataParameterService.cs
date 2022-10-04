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
        }

        public decimal? GetDataParameterValue<TModel>(DataParameter formParameter, IDictionary<int, object> userParametersValue, TModel model)
        {
            if (formParameter.ParameterType == DataParameterType.EntityProperties)
            {
                if (model != null)
                {
                    var info = typeof(TModel).GetProperty(formParameter.PropertyName);
                    var value = info.GetValue(model);
                    if (value == null)
                        return null;
                    return Convert.ToDecimal(value);
                }
                return null;
            }
            if (userParametersValue.ContainsKey(formParameter.DynamicParameterId.Value))
                return Convert.ToDecimal(userParametersValue[formParameter.DynamicParameterId.Value]);
            return null;
            
        }
    }
}
