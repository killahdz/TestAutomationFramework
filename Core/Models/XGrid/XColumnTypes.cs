namespace Core.Models.XGrid
{
    /// <summary>
    ///     Take note of XColumnTypePrefix value
    ///     This is the prefix that should be specified in the input data to indicate how to handle this input
    ///     If no prefix is specified, Default is assumed.
    /// </summary>
    public enum XColumTypes
    {
        [XColumnTypePrefix("tx")] Default,

        [XColumnTypePrefix("li")] Select,

        [XColumnTypePrefix("cb")] Combo,

        [XColumnTypePrefix("dt")] DateAndTime,

        [XColumnTypePrefix("ck")] Checkbox,
    }
}