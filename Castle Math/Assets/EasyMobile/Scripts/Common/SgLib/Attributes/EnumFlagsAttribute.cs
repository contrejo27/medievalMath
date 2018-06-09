using UnityEngine;
using System;

namespace SgLib.Attributes
{
    public class EnumFlagsAttribute : PropertyAttribute
    {
        public string enumName;

        public EnumFlagsAttribute()
        {
        }

        public EnumFlagsAttribute(string name)
        {
            enumName = name;
        }
    }
}