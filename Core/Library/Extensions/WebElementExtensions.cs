using Core.Library.WebDriver;

namespace Core.Library.Extensions
{
    public static class WebElementExtensions
    {
        /// <summary>
        ///     Clicks and scrolls to
        /// </summary>
        /// <param name="element"></param>
        public static void ClickTo(this ProxiedWebElement element)
        {
            element.ScrollIntoView();
            element.Click();
        }

        /// <summary>
        ///     Scrolls, clears and sends keys to element
        /// </summary>
        /// <param name="element"></param>
        /// <param name="val"></param>
        public static void SendKeysTo(this ProxiedWebElement element, string val, bool clear = false)
        {
            element.ScrollIntoView();

            if (clear)
                element.Clear();

            element.SendKeys(val);
        }
    }
}