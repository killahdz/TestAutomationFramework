using System;

namespace Core.Library.Helpers
{
    public static class DateHelpers
    {
        private static readonly string DateFormat = "dddd dd MMMM yyyy";

        public static string YesterdayAsString => DateTime.Now.AddDays(-1).ToString(DateFormat);
    }
}