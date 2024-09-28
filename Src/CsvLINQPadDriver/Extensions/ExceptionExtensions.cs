using System;
using System.Threading;

namespace CsvLINQPadDriver.Extensions;

internal static class ExceptionExtensions
{
    public static bool CanBeHandled(this Exception exception) =>
        exception is not (
            AccessViolationException or
            ArgumentException        or
            IndexOutOfRangeException or
            NullReferenceException   or
            OutOfMemoryException     or
            StackOverflowException   or
            ThreadAbortException
        );
}
