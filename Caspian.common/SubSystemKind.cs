using System.Reflection;
using Caspian.Common.Attributes;

namespace Caspian.Common
{
    public enum SubSystemKind: byte
    { 
        /// <summary>
        /// بخش اصلی
        /// </summary>
        [EnumField("Engin")]
        Engine = 1,

        /// <summary>
        /// دمو
        /// </summary>
        [EnumField("دمو")]
        Demo,

        /// <summary>
        /// کارگزینی
        /// </summary>
        [EnumField("کارگزینی")]
        Employment,

        /// <summary>
        /// کارتابل
        /// </summary>
        [EnumField("کارتابل")]
        Kartable
    }

    public static class SubSystemExt
    {
        private static Assembly GetAssembly(SubSystemKind subSystemKind, bool isModel)
        {
            var path = Assembly.GetExecutingAssembly().Location;
            var index = path.LastIndexOf("\\");
            path = path.Substring(0, index) + "\\";
            path += subSystemKind.ToString() + (isModel ? ".Model" : ".Service") + ".dll";
            return Assembly.LoadFile(path);
        }

        public static Assembly GetEntityAssembly(this SubSystemKind systemKind)
        {
            return SubSystemExt.GetAssembly(systemKind, true);
        }

        public static Assembly GetServiceAssembly(this SubSystemKind systemKind)
        {
            return SubSystemExt.GetAssembly(systemKind, false);
        }
    }
}
