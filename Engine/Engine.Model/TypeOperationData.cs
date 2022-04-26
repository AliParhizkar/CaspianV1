namespace Caspian.Engine
{
    public class TypeOperationData
    {
        public ValueTypeKind FirstType { get; set; }

        public ValueTypeKind SecondType { get; set; }

        public OperatorType[] OperatorsType { get; set; }

        public ValueTypeKind Result { get; set; }
    }
}
