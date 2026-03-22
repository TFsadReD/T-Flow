using System;

namespace TFlow.Core
{
    public class ErrorManager
    {
        private string _error;
        private string _result;
        private int _line;

        public ErrorManager(string error, string result, int line)
        {
            _error = error;
            _result = result;
            _line = line;
        }

        public void Report() => Console.WriteLine($"Error: {_error} >>> {_result}\nLine: {_line}");
    }
}
