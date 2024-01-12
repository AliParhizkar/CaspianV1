﻿using System.Xml.Linq;

namespace Caspian.Common
{

    public class Column
    {
        public Column(string name, Type type)
        {
            Name = name;
            Type = type;
        }

        public string Name { get; private set; }

        public Type Type { get; private set; }
    }
}