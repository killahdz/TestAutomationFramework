using System;

namespace Core.Library.Helpers
{
    public static class DataHelpers
    {
        public static string RandomPhone
        {
            get
            {
                var random = new Random();
                return string.Format("04{0}{1}", random.Next(1111, 9999), random.Next(1111, 9999));
            }
        }

        public static string RandomEmail => string.Format("{0}@test.com", Guid.NewGuid().ToString());
    }
}