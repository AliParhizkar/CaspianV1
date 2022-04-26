﻿using System.Reflection;
using Caspian.Common.Attributes;

namespace Caspian.Common
{
    public enum SubSystemKind: byte
    { 
        /// <summary>
        /// بخش اصلی
        /// </summary>
        [EnumField("Engin")]
        Engin = 1,

        [EnumField("دمو")]
        Demo,

        [EnumField("کارگزینی")]
        Employment
    }

    public static class SubSystemExt
    {
        private static Assembly GetAssembly(SubSystemKind subSystemKind, bool isModel)
        {
            var path = Assembly.GetExecutingAssembly().Location;
            var index = path.LastIndexOf("\\");
            path = path.Substring(0, index) + "\\";
            path += subSystemKind.ToString() + (isModel ? "Model" : "Service") + ".dll";
            return Assembly.LoadFile(path);
        }

        public static Assembly GetServiceAssembly(this SubSystemKind systemKind)
        {
            return SubSystemExt.GetAssembly(systemKind, false);
        }
    }
}
