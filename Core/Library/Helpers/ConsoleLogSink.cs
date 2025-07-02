using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Core.Library.Helpers
{
    internal class ConsoleLogSink : IDisposable
    {
        private readonly TextWriter _oldError;
        private readonly IntPtr _oldErrorHandle;

        private readonly TextWriter _oldOut;
        private readonly IntPtr _oldOutHandle;

        public ConsoleLogSink()
        {
            _oldOutHandle = GetStdHandle(-11);
            _oldErrorHandle = GetStdHandle(-12);
            _oldOut = Console.Out;
            _oldError = Console.Error;
            Console.SetOut(TextWriter.Null);
            Console.SetError(TextWriter.Null);
            SetStdHandle(-11, IntPtr.Zero);
            SetStdHandle(-12, IntPtr.Zero);
        }

        public void Dispose()
        {
            SetStdHandle(-11, _oldOutHandle);
            SetStdHandle(-12, _oldErrorHandle);
            Console.SetOut(_oldOut);
            Console.SetError(_oldError);
        }

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll")]
        public static extern int SetStdHandle(int nStdHandle, IntPtr hHandle);
    }
}