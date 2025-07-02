using System;
using System.Linq;
using Core.Library.Extensions;
using Core.Library.WebDriver;

namespace Core.Models.XGrid
{
    /// <summary>
    ///     Dumb object for use by XGrid
    ///     Represents a single column in an XGrid
    /// </summary>
    public class XColumn
    {
        public int Index { get; set; }
        public string ColumnId { get; set; }
        public Action<XColumn> InputHandler { get; set; }
        public ProxiedWebElement HeaderElement { get; set; }

        /// <summary>
        ///     set via SetData. Other props are derived from this value
        /// </summary>
        private string RawData { get; set; }

        /// <summary>
        ///     RawData with prefixes removed
        /// </summary>
        public string InputData
        {
            get
            {
                if (RawData == null)
                    return null;

                //do any type specific stuff here
                var prefix = $"{ColumnType.GetEnumAttribute<XColumnTypePrefixAttribute>().Prefix}:";
                var inputData = RawData.Replace(prefix, "");
                return inputData;
            }
        }

        public bool BoolValue => InputData == "1";

        public string DateValue
        {
            get
            {
                try
                {
                    var numDays = int.Parse(InputData);
                    return DateTime.Now.AddDays(numDays).ToString(XGrid.DateFormat);
                }
                catch (Exception)
                {
                    return DateTime.Now.ToString(XGrid.DateFormat);
                }
            }
        }

        public string TimeValue
        {
            get
            {
                try
                {
                    var numDays = int.Parse(InputData);
                    return DateTime.Now.AddDays(numDays).ToString(XGrid.TimeFormat);
                }
                catch (Exception)
                {
                    return DateTime.Now.ToString(XGrid.TimeFormat);
                }
            }
        }

        /// <summary>
        ///     derives the column type based on the raw data
        /// </summary>
        public XColumTypes ColumnType
        {
            get
            {
                try
                {
                    return Enum.GetValues(typeof(XColumTypes))
                        .Cast<XColumTypes>()
                        .FirstOrDefault(e =>
                            RawData.StartsWith($"{e.GetEnumAttribute<XColumnTypePrefixAttribute>().Prefix}:"));
                }
                catch (Exception)
                {
                    return XColumTypes.Default;
                }
            }
        }

        /// <summary>
        ///     Skip this column if no data present
        /// </summary>
        public bool Skip => string.IsNullOrEmpty(RawData);

        public void SetData(string data)
        {
            RawData = data?.Trim();
        }
    }
}