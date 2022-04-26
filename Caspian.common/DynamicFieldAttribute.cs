using System;

namespace Caspian.Engine
{
    /// <summary>
    /// برای مشخص کردن یک <see cref="Type"/> یعنوان نوع پویا مورد استفاده قرار می گیرد.
    /// </summary>
    public class DynamicFieldAttribute : Attribute
    {
        public Type ParameterType { get; set; }

        public Type ParameterValueType { get; set; }

        public Type OtherType { get; private set; }

        public string FaTitle { get; private set; }

        public DynamicFieldAttribute(Type type)
        {
            ParameterValueType = type;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="faTitle"></param>
        public DynamicFieldAttribute(Type type, string faTitle)
        {
            OtherType = type;
            FaTitle = faTitle;
        }

        public DynamicFieldAttribute(Type parameterType, Type parameterValueType)
        {
            ParameterType = parameterType;
            ParameterValueType = parameterValueType;
        }
    }
}
