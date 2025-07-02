using System;
using System.Collections.Generic;
using System.Text;
using OpenQA.Selenium;

namespace Core.Library.Exceptions
{
    public class ScriptErrorException : Exception
    {
        public ScriptErrorException(string message, ScriptErrorScope scope, string logLevel = "n/a") : base(message)
        {
            Scope = scope;
            LogLevel = logLevel;
        }

        public ScriptErrorException(List<LogEntry> errorLogs)
        {
            Logs = errorLogs;
        }

        public ScriptErrorScope Scope { get; set; }
        public string LogLevel { get; set; }
        public List<LogEntry> Logs { get; set; }

        public override string ToString()
        {
            if (Logs == null)
            {
                return $"Script error detected. Scope: {Scope}, Level: {LogLevel}, Message: {Message}";
            }
            else
            {
                if (Logs.Count == 1)
                    return
                        $"Script error detected. Scope: {ScriptErrorScope.Page}, Level: {Logs[0].Level}, Message: {Logs[0].Message}";
                //aggregate the messages
                var message = new StringBuilder($"{Logs.Count} script errors detected.");
                message.AppendLine("========================================");
                for (var i = 0; i < Logs.Count; i++)
                    message.AppendLine(
                        $"[{i}] Scope: {ScriptErrorScope.Page}, Level: {Logs[i].Level}, Message: {Logs[i].Message}");

                return message.ToString();
            }
        }
    }

    public enum ScriptErrorScope
    {
        Page,
        Form,
        Field
    }
}