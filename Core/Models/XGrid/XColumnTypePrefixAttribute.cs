using System;

namespace Core.Models.XGrid
{
    public class XColumnTypePrefixAttribute : Attribute
    {
        public XColumnTypePrefixAttribute(string prefix = null)
        {
            Prefix = prefix;
        }

        public string Prefix { get; }
    }
}