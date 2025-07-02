using System.Collections.Generic;
using Newtonsoft.Json;

namespace Core.Library.Mapping
{
    public class NameMap
    {
        public string PrettyName { get; set; }

        public string RealName { get; set; }

        public static List<NameMap> Parse(string json)
        {
            return JsonConvert.DeserializeObject<List<NameMap>>(json);
        }
    }
}