using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Core.Library.Extensions;
using Core.Library.WebDriver;
using Core.Models.Portal;
using OpenQA.Selenium;
using TechTalk.SpecFlow;
using Keys = OpenQA.Selenium.Keys;

namespace Core.Models.XGrid
{
    /// <summary>
    ///     Functionality to handle inputing data into an XGrid
    ///     See <see cref="XColumTypes" /> enum for supported input types
    ///     See Handlexxxx methods for input implementation
    ///     Usage:
    ///     1.  Create new instance of XGrid specifying Name Map Label
    ///     2.  Call Parse() method providing raw input data
    /// </summary>
    public class XGrid : PortalFormsModel
    {
        public static string DateFormat = "dd/MM/yyyy";
        public static string TimeFormat = "hh:mm";

        /// <summary>
        ///     Ctor
        /// </summary>
        /// <param name="context">requried</param>
        /// <param name="label">Name map label. required</param>
        /// <param name="idTransformFunc">Use when the element Ids in your form or page context use different id prefixes</param>
        public XGrid(ScenarioContext context, string label, Func<string, string> idTransformFunc = null) : base(context)
        {
            //get the root container of the grid
            GridNameMapLabel = label;

            if (idTransformFunc != null)
                _containerIdTransformFunc = idTransformFunc;

            var colIndex = 0;
            //get the columns
            Columns = GridContainer.FindElements(ColumnHeaders)
                .Where(e => e.Displayed)
                .Select(e => new XColumn
                {
                    Index = ++colIndex,
                    ColumnId = e.GetAttribute("id"),
                    HeaderElement = e
                }).ToList();
        }

        /// <summary>
        ///     Parses the data and iterates each column plugging in data at each input
        /// </summary>
        /// <param name="values"></param>
        private void AddToRow(string values)
        {
            // var sw = Stopwatch.StartNew();

            if (string.IsNullOrEmpty(values))
                throw new NotImplementedException("AddToRow. values is null or empty");

            //sw.Restart();
            //parse the gridvals
            Parse(values);
            //Driver.RecordLog($"After Parse() elapsed: {sw.ElapsedMilliseconds}");

            //iterate the columns and put the values in
            foreach (var col in Columns)
            {
                //if no data to input, skip to the next column
                if (col.Skip) continue;

                //if this is a new row and first cell we should already have the input activated
                //Important we dont try interact with the trigger cell in this case because it will deactivate the input
                //For all other cases, we need to click the trigger cell
                if (IsDirty || !IsNewRow)
                {
                    //sw.Restart();
                    var triggerCell = GetTriggerCell(col);
                    // Driver.RecordLog($"After GetTriggerCell() elapsed: {sw.ElapsedMilliseconds}");

                    //Clicking the trigger cell in the grid row will move an input field to be absolutely positioned at the trigger cell
                    //The input will take focus
                    //  sw.Restart();
                    triggerCell.Click();
                    try
                    {
                        triggerCell.ScrollIntoView();
                    }
                    catch (StaleElementReferenceException)
                    {
                        GetTriggerCell(col).ScrollIntoView();
                    }
                    // Driver.RecordLog($"After triggerCell.ClickTo() elapsed: {sw.ElapsedMilliseconds}");

                    ShortWait();
                }

                //sw.Restart();
                //interact with the input
                col.InputHandler(col);
                //Driver.RecordLog($"After  col.InputHandler() elapsed: {sw.ElapsedMilliseconds}");

                //mark as dirty
                IsDirty = true;
            }
        }

        /// <summary>
        ///     Gets the trigger cell for the current column and targetrow
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        private ProxiedWebElement GetTriggerCell(XColumn column)
        {
            var triggerCell = TargetRow.FindElement(By.ClassName($"{_gridCellIdPrefix}{column.ColumnId}"));
            if (triggerCell == null)
                throw new ApplicationException($"Could not locate trigger cell for {column.ColumnId}");

            return triggerCell;
        }

        /// <summary>
        ///     Splits input vals, parses for types and checks column count
        ///     assigns input handlers based on column type
        /// </summary>
        /// <param name="values"></param>
        private void Parse(string values)
        {
            //check for empty
            if (string.IsNullOrEmpty(values))
                throw new NotImplementedException("Must specify grid values");

            //split the vals on delimiter
            var vals = values.Split(_valDelimiter);

            //validate num vals agaisnt num columns
            if (vals.Count() != Columns.Count)
                throw new NotImplementedException(
                    $"Value count ({vals.Count()} does not match Column count ({Columns.Count}). Pleae ensure placeholders are used in the input data to account for uneditable columns");

            for (var i = 0; i < Columns.Count; i++) Columns[i].SetData(vals[i]);

            //assign input handlers
            Columns.ForEach(c =>
            {
                if (c.ColumnType == XColumTypes.Checkbox)
                    c.InputHandler = HandleCheckboxInput;
                else if (c.ColumnType == XColumTypes.Combo)
                    c.InputHandler = HandleComboInput;
                else if (c.ColumnType == XColumTypes.DateAndTime)
                    c.InputHandler = HandleDateTimeInput;
                else if (c.ColumnType == XColumTypes.Select)
                    c.InputHandler = HandleSelectInput;
                else
                    c.InputHandler = HandleTextInput;
            });
        }

        #region Private props

        private List<XColumn> Columns { get; }
        private bool IsNewRow { get; set; }
        private bool IsDirty { get; set; }
        private string GridNameMapLabel { get; }

        private static readonly By ListLoadingProgressMask = By.ClassName("x-mask-msg");
        private static readonly By SuggestionList = By.ClassName("x-boundlist");
        private static readonly By SuggestionListItems = By.ClassName("x-boundlist-item");
        private static readonly By EditableGridRows = By.ClassName("x-grid-row");
        private static readonly By ColumnHeaders = By.ClassName("x-column-header");
        private static readonly By CheckboxContainer = By.ClassName("x-grid-checkheader");

        private static readonly string _gridCellIdPrefix = "x-grid-cell-";
        private static readonly char _valDelimiter = ';';
        private static readonly string _checkboxIsCheckedClass = "x-grid-checkheader-checked";

        private Func<string, string> _containerIdTransformFunc = (id) =>
        {
            return $"container_{id.Replace("IO:", "")}";
        };

        #endregion Private props

        #region Access Props

        /// <summary>
        ///     Parent container for the entire grid
        /// </summary>
        public ProxiedWebElement GridContainer
        {
            get
            {
                if (GridNameMapLabel == null) return null;

                try
                {
                    return GetTargetByNameMapLabel(GridNameMapLabel, idTransformFunc: _containerIdTransformFunc);
                }
                catch (StaleElementReferenceException)
                {
                    return GetTargetByNameMapLabel(GridNameMapLabel, idTransformFunc: _containerIdTransformFunc);
                }
            }
        }

        /// <summary>
        ///     The row we are currently working on
        ///     Will differ depending on whether a new row has been created or working in existing row
        /// </summary>
        public ProxiedWebElement TargetRow
        {
            get
            {
                if (GridContainer == null) return null;
                if (IsNewRow)
                    return GridRows.Last();
                return GridRows.First();
            }
        }

        /// <summary>
        ///     The add row button
        /// </summary>
        private ProxiedWebElement AddRowButton
        {
            get
            {
                var addButton = GridContainer.FindElements(By.TagName("button"))
                    .FirstOrDefault(b => b.Text.IsSimilarTo("add"));
                if (addButton == null)
                    throw new ApplicationException("Could not find add row button");

                return addButton;
            }
        }

        /// <summary>
        ///     The remove row button
        /// </summary>
        private ProxiedWebElement RemoveRowButton
        {
            get
            {
                var removeButton = GridContainer.FindElements(By.TagName("button"))
                    .FirstOrDefault(b => b.Text.IsSimilarTo("remove"));
                if (removeButton == null)
                    throw new ApplicationException("Could not find remove row button");

                return removeButton;
            }
        }

        /// <summary>
        ///     The Copy button
        /// </summary>
        private ProxiedWebElement CopyButton
        {
            get
            {
                var copyButton = GridContainer.FindElements(By.TagName("button"))
                    .FirstOrDefault(b => b.Text.IsSimilarTo("Copy"));
                if (copyButton == null)
                    throw new ApplicationException("Could not find Copy button");

                return copyButton;
            }
        }

        /// <summary>
        ///     Grid rows
        /// </summary>
        private IEnumerable<ProxiedWebElement> GridRows
        {
            get
            {
                try
                {
                    return GridContainer.FindElements(EditableGridRows);
                }
                catch (StaleElementReferenceException)
                {
                    //try again
                    return GridContainer.FindElements(EditableGridRows);
                }
            }
        }

        #endregion Access Props

        #region Public methods


        public bool VerifyGridData(string expectedData)
        {
            //execute javascript to check the grid for the expected field values
            var script = @"
                /**
                * Verifies that every value in expectedItems, exists in a grid row belonging to the grid specified by gridName
                * @param {string} gridName 
                * @param {Array<string>} expectedItems - a row of items we expect to find in the grid
                */
                function verifyGridData(gridName, expectedItems) {

                    var allItemsFound = false;
                    //iterate grid rows
                    getGridForVariable(gridName).store.data.items.each(function (item) {
                        var expectedItemFoundCount = 0;
                        //iterate expected items
                        expectedItems.each(function (expectedItem) {
                            for (let property in item.raw) {
                                if (item.raw.hasOwnProperty(property)) {
                                    if (item.raw[property].toString().startsWith(expectedItem))
                                        expectedItemFoundCount++;
                                }
                            }
                        });
                        if (expectedItemFoundCount === expectedItems.length) {
                            allItemsFound = true;
                            return;
                        }
                    });
                    return allItemsFound;
                }
            ";
            //inject the script

            //expected data should be a semicolon separated list
            var data = $"'{expectedData.Replace(_valDelimiter.ToString(), "', '")}'";
            //append the call to the 
            script += $"return verifyGridData('{GridNameMapLabel}', [{data}]);";

            var result = Driver.ExecuteScript<bool>(script);

            return result;
        }

    

        /// <summary>
        ///     Clicks the remove row button n times
        /// </summary>
        /// <param name="n">default 3</param>
        public void RemoveRows(int n = 3)
        {
            var removeButton = RemoveRowButton;

            for (var i = 0; i < n; i++) removeButton.Click();
        }

        /// <summary>
        ///     Adds a new row to the grid and attempts to set each columns values
        /// </summary>
        /// <param name="values"></param>
        public void AddNewRow(string values)
        {
            //locate and click to the row button
            AddRowButton.ClickTo();

            //wait for rows
            Driver.WaitUntil((driver) => { return GridRows != null && GridRows.Any(); });
            
            //mark as new row
            IsNewRow = true;

            //process the data
            AddToRow(values);
        }

        /// <summary>
        ///     Adds values to an existing row
        /// </summary>
        /// <param name="values"></param>
        public void AddToExistingRow(string values)
        {
            AddToRow(values);
        }

        #endregion Public methods

        #region Input Handlers

        /// <summary>
        ///     Default handler - simply sends keys to the input
        /// </summary>
        /// <param name="col"></param>
        private void HandleTextInput(XColumn col)
        {
            //set the data
            ActiveElement.SendKeys($"{col.InputData}");
        }

        /// <summary>
        ///     Checkbox Handler
        /// </summary>
        /// <param name="col"></param>
        private void HandleCheckboxInput(XColumn col)
        {
            //clicking the trigger cell should have toggled the checkbox
            //we need to check the current value aligns with the intended value

            //get the indicator from the trigger cell
            var isChecked = GetTriggerCell(col).FindElement(CheckboxContainer)
                .GetAttribute("class").Contains(_checkboxIsCheckedClass);

            //toggle the value if needed
            if (isChecked != col.BoolValue)
                CurrentFocusProxy.SendKeys(Keys.Space).Perform();
        }

        /// <summary>
        ///     Date and Time input handler
        ///     Does not handle date or time individually
        /// </summary>
        /// <param name="col"></param>
        private void HandleDateTimeInput(XColumn col)
        {
            //click the input
            ActiveElement.Click();
            ShortWait();

            //focus should be in the date input, set the date
            ActiveElement.SendKeys(col.DateValue);
            //navigate to the time input
            ActiveElement.SendKeys(Keys.Tab);
            //set the time
            ActiveElement.SendKeys(col.TimeValue);
        }

        /// <summary>
        ///     Combo box with suggestion handler
        ///     Sends keys then selects the first matching item from the suggestion list
        ///     Say your target item is something like "1000101010 My Awesome Code", specifying a value of 1000101010 will select
        ///     the first match
        ///     So if there are other items beginnning with "1000101010 eg.  "1000101010 another awesome Code", the unintended
        ///     option may get selected
        ///     Be as specific as the field allows
        /// </summary>
        /// <param name="col"></param>
        private void HandleComboInput(XColumn col)
        {
            //click the input
            ActiveElement.Click();
            ShortWait();

            //enter input data
            ActiveElement.Clear();
            ActiveElement.SendKeys(col.InputData);

            //wait for list to show
            Driver.WaitUntilElementNotVisible(ListLoadingProgressMask);
            Driver.WaitUntilElementVisible(SuggestionListItems);
            ShortWait();

            //select the highlighted element
            ActiveElement.SendKeys(Keys.Enter);
        }

        /// <summary>
        ///     Handles a select input. Specified value must exist in the list
        /// </summary>
        /// <param name="col"></param>
        private void HandleSelectInput(XColumn col)
        {
            //click the input so the options appear
            ActiveElement.Click();
            ShortWait();

            //wait for items to load
            Driver.WaitUntilElementNotVisible(ListLoadingProgressMask);
            Driver.WaitUntilElementVisible(SuggestionListItems);

            //get all the list options
            var list = Driver.FindElements(SuggestionListItems)
                .Where(e => e.Displayed);

            if (list == null)
                throw new ApplicationException($"Could not locate x-boundlist");

            //iterate the items using the down key until we locate our target
            var listItemFound = false;
            foreach (var listItem in list)
                if (listItem.Text.IsSimilarTo(col.InputData))
                {
                    //found our item
                    Driver.WaitUntilElementNotVisible(ListLoadingProgressMask);
                    //select it
                    CurrentFocusProxy.SendKeys(Keys.Enter).Perform();
                    //mark as found
                    listItemFound = true;
                    break;
                }
                else
                {
                    //go down to next item
                    CurrentFocusProxy.SendKeys(Keys.ArrowDown).Perform();
                }

            //throw exception if item not found
            if (!listItemFound)
                throw new NotImplementedException(
                    $"Could no locate listItem with text '{col.InputData}' for column ({col.Index}).");
        }

        #endregion Input Handlers
    }
}