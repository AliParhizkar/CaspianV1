using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Caspian.Engine.Service
{
    public class DynamicFormCalculationService<TModel>
    {
        TModel model;
        IDictionary<int, object> userParametersValue;
        IList<DynamicParameter> formParameters;
        IList<DataParameterValue> parameterValues;


        public DynamicFormCalculationService(IDictionary<int, object> userParametersValue, TModel model)
        {
            this.model = model;
            this.userParametersValue = userParametersValue;
        }

        public async Task InitializeAsync(IServiceScope scope)
        {
            formParameters = await new DynamicParameterService(scope).GetAll().Where(t => t.CalculationType == CalculationType.FormData).Include(t => t.ResultParameters)
                .ToListAsync();
            parameterValues = await new DataParameterValueService(scope).GetAll().ToListAsync();
        }

        public IDictionary<int, decimal?> GetFormData()
        {
            var formData = new Dictionary<int, decimal?>();
            foreach (var formParameter in formParameters)
            {
                var dic = new Dictionary<int, int>();
                foreach (var param in formParameter.ResultParameters)
                {
                    var value = new DataParameterService(null).GetDataParameterValue(param, userParametersValue, model);
                    dic.Add(param.Id, Convert.ToInt32(value));
                }
                var result = parameterValues.SingleOrDefault(GetFunc(dic))?.ResultValue;
                formData.Add(formParameter.Id, result);
            }
            return formData;
        }

        Func<DataParameterValue, bool> GetFunc(IDictionary<int, int> dic)
        {
            switch(dic.Count)
            {
                case 1:
                    return t => t.Parameter1Id == dic.ElementAt(0).Key && t.Value1 == dic.ElementAt(0).Value;
                case 2:
                    return t => t.Parameter1Id == dic.ElementAt(0).Key && t.Value1 == dic.ElementAt(0).Value &&
                        t.Parameter2Id == dic.ElementAt(1).Key && t.Value2 == dic.ElementAt(1).Value;
                case 3:
                    return t => t.Parameter1Id == dic.ElementAt(0).Key && t.Value1 == dic.ElementAt(0).Value &&
                        t.Parameter2Id == dic.ElementAt(1).Key && t.Value2 == dic.ElementAt(1).Value &&
                        t.Parameter3Id == dic.ElementAt(2).Key && t.Value3 == dic.ElementAt(2).Value;
                case 4:
                    return t => t.Parameter1Id == dic.ElementAt(0).Key && t.Value1 == dic.ElementAt(0).Value &&
                        t.Parameter2Id == dic.ElementAt(1).Key && t.Value2 == dic.ElementAt(1).Value &&
                        t.Parameter3Id == dic.ElementAt(2).Key && t.Value3 == dic.ElementAt(2).Value &&
                        t.Parameter4Id == dic.ElementAt(3).Key && t.Value4 == dic.ElementAt(3).Value;
                default:
                    throw new NotImplementedException("خطای عدم پیاده سازی");
            }
        }
    }
}