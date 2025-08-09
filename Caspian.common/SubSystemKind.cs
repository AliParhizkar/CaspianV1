using System.Reflection;
using System.ComponentModel.DataAnnotations;

namespace Caspian.Common
{
    public enum SubSystemKind: byte
    { 
        /// <summary>
        /// بخش اصلی
        /// </summary>
        [Display(Name = "Engine")]
        Engine = 1,

        /// <summary>
        /// دمو
        /// </summary>
        [Display(Name = "دمو")]
        Demo,

        [Display(Name = "فروش")]
        Marketing,

        [Display(Name = "انبار")]
        Warehouse
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

        public static bool HasEntityType(this SubSystemKind systemKind, string namespace_, string name)
        {
            return GetEntityAssembly(systemKind).GetTypes().Any(t => t.Namespace == namespace_ && t.Name == name); 
        }

        public static Assembly GetServiceAssembly(this SubSystemKind systemKind)
        {
            return SubSystemExt.GetAssembly(systemKind, false);
        }
    }
}
