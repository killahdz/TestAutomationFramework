using System;

namespace Core.Library.WebDriver
{
    public class BrowserProcessAttribute : Attribute
    {
        public BrowserProcessAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}