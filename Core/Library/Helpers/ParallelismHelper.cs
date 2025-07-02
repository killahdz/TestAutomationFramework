using System.Reflection;
using NUnit.Framework;

namespace Core.Library.Helpers
{
    public class ParallelismHelper
    {
        /// <summary>
        ///     Detects whether the assembly is in Parallel mode
        /// </summary>
        /// <returns></returns>
        public static bool IsInParallel()
        {
            var entryAssembly = Assembly.GetCallingAssembly();

            var attributes = entryAssembly.GetCustomAttributes(typeof(LevelOfParallelismAttribute), false);

            LevelOfParallelismAttribute attribute = null;
            if (attributes.Length > 0) attribute = attributes[0] as LevelOfParallelismAttribute;

            if (attribute != null && attribute.Properties.ContainsKey("LevelOfParallelism"))
            {
                var props = attribute.Properties["LevelOfParallelism"];
                if (props.Count > 0)
                {
                    var degrees = (int) attribute.Properties["LevelOfParallelism"][0];

                    return degrees > 1;
                }
            }

            return false;
        }
    }
}