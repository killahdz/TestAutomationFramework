using System.Collections.Generic;
using System.Linq;

namespace Core.Library.Exceptions
{
    public class ScriptErrors
    {
        public static List<string> Ignore = new List<string>
        {
            //https://rttmsdev.service-now.com/angular.min.js.jsdbx?c=2018-01-04.7 117:9 "Error: [$compile:tpload] http://errors.angularjs.org/1.5.8/$compile/tpload?p0=%2Frttms%2Fportal.ticket-status-drop.tpl.html.do%3Fsysparm_direct%3Dtrue%26c%3D1515042076768&p1=-1&p2=\n    at https://rttmsdev.service-now.com/angular.min.js.jsdbx?c=2018-01-04.7:6:412\n    at https://rttmsdev.service-now.com/angular.min.js.jsdbx?c=2018-01-04.7:156:511\n    at https://rttmsdev.service-now.com/angular.min.js.jsdbx?c=2018-01-04.7:131:20\n    at m.$eval (https://rttmsdev.service-now.com/angular.min.js.jsdbx?c=2018-01-04.7:145:347)\n    at m.$digest (https://rttmsdev.service-now.com/angular.min.js.jsdbx?c=2018-01-04.7:142:420)\n    at m.$apply (https://rttmsdev.service-now.com/angular.min.js.jsdbx?c=2018-01-04.7:146:113)\n    at l (https://rttmsdev.service-now.com/angular.min.js.jsdbx?c=2018-01-04.7:97:322)\n    at J (https://rttmsdev.service-now.com/angular.min.js.jsdbx?c=2018-01-04.7:102:34)\n    at XMLHttpRequest.e (https://rttmsdev.service-now.com/angular.min.js.jsdbx?c=2018-01-04.7:103:55)"
            //"$compile:tpload"
        };

        public static bool CanIgnore(string message)
        {
            return Ignore.Any(i => message.Contains(i));
        }
    }
}