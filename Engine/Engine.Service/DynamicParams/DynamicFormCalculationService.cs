namespace Caspian.Engine.Service
{
    public class DynamicFormCalculationService
    {
        object model;
        IDictionary<int, object> userParametersValue;
        IList<DataParameterValue> parameterValues;

        public DynamicFormCalculationService(IDictionary<int, object> userParametersValue, object model)
        {
            this.model = model;
            this.userParametersValue = userParametersValue;
        }



    }
}