using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Core.Library.Helpers
{
    /// <summary>
    ///     Helper class for handling orphan processes
    /// </summary>
    public class ProcessHelper
    {
        private readonly IEnumerable<int> _preExistingProcesses = new List<int>();

        public ProcessHelper(string processName)
        {
            ProcessName = processName;
            _preExistingProcesses = Process.GetProcessesByName(ProcessName).Select(p => p.Id);
        }

        public string ProcessName { get; }

        /// <summary>
        ///     Kills all processes by the name this instance was initailised with
        /// </summary>
        /// <param name="killAll">Specify false if you wish to preserve pre-existing processes of the same name</param>
        public void Kill(bool killAll = true)
        {
            var processesToKill = Process.GetProcessesByName(ProcessName).Select(p => p.Id);

            if (!killAll)
                processesToKill = _preExistingProcesses.Except(processesToKill)
                    .Union(processesToKill.Except(_preExistingProcesses));

            foreach (var process in processesToKill)
                try
                {
                    Process.GetProcessById(process).Kill();
                }
                catch
                {
                    //TODO: log something
                }
        }
    }
}