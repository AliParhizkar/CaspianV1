using Caspian.Common.Extension;

namespace Caspian.Engine.Extension
{
    public static class EngineExtension
    {
        public static ValueTypeKind GetValueTypeKind(this Type type)
        {
            type = type.GetUnderlyingType();
            var integerTypes = new Type[] { typeof(byte), typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(long), typeof(ulong) };
            if (integerTypes.Contains(type))
                return ValueTypeKind.Int;
            if (type.IsEnumType())
                return ValueTypeKind.Enum;
            if (type == typeof(bool))
                return ValueTypeKind.Bool;
            if (type == typeof(DateOnly))
                return ValueTypeKind.Date;
            if (type  == typeof(TimeOnly))
                return ValueTypeKind.Time;
            var realTimes = new Type[] { typeof(double), typeof(float), typeof(decimal) };
            if (realTimes.Contains(type))
                return ValueTypeKind.Double;
            if (type == typeof(string))
                return ValueTypeKind.String;
            throw new NotImplementedException("خطای عدم پیاده سازی");
        }
    }
}
