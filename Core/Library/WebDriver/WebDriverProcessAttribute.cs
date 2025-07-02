using System;

namespace Core.Library.WebDriver
{
    public class WebDriverProcessAttribute : Attribute
    {
        public WebDriverProcessAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}