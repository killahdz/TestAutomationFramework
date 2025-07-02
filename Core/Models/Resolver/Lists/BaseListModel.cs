using System;
using System.Linq;
using Core.Library;
using Core.Library.WebDriver;
using OpenQA.Selenium;
using TechTalk.SpecFlow;

namespace Core.Models.Resolver.Lists
{
    public class BaseListModel : BaseModel
    {
        public static readonly By BodyContentDetail = By.ClassName("list_div_cell");
        public static readonly By ListRows = By.ClassName("list_row");
        public static readonly By ListFormLink = By.ClassName("formlink");

        public BaseListModel(ScenarioContext context) : base(context)
        {
        }

        /// <summary>
        ///     Returns a random row from all displayed rows
        /// </summary>
        /// <returns></returns>
        private ProxiedWebElement GetRandomRow()
        {
            var rows = Driver.FindElements(ListRows);
            if (rows == null || !rows.Any())
                throw new NotImplementedException("No list rows found");

            var random = new Random(DateTime.Now.Millisecond);
            var row = rows[random.Next(0, rows.Count - 1)];

            return row;
        }

        /// <summary>
        ///     Returns the first row from all displayed rows
        /// </summary>
        /// <returns></returns>
        private ProxiedWebElement GetFirstRow()
        {
            var rows = Driver.FindElements(ListRows);
            if (rows == null || !rows.Any())
                throw new NotImplementedException("No list rows found");
            
            return rows.First();
        }

        /// <summary>
        ///     Will navigate to a random list item
        /// </summary>
        public void OpenRandomRow()
        {
            var row = GetRandomRow();
            var anchor = row.FindElement(ListFormLink);

            Driver.Navigation.GoToUrl(anchor.GetAttribute("href"));
        }

        /// <summary>
        ///     Opens the first row item
        /// </summary>
        public void OpenFirstRow()
        {
            var row = GetFirstRow();
            var anchor = row.FindElement(ListFormLink);

            Driver.Navigation.GoToUrl(anchor.GetAttribute("href"));
            ShortWait(2000);
            Driver.WaitUntiPageLoaded();
        }

        /// <summary>
        ///     Navigate to a List
        /// </summary>
        /// <param name="url"></param>
        public void GoToList(string url)
        {
            Driver.Retry(()=> Browse(ScenarioContext, BodyContentDetail, Urls.GetUrl(url), ConfigManager.ShortWait), 
                $"An error occured while navigating to resolver list at url {url}", 2);
        }
    }
}