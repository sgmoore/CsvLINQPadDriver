using System;
using System.ComponentModel;
using System.Threading;

namespace CsvLINQPadDriver.Extensions;

internal static class ExceptionExtensions
{
    public static bool CanBeHandled(this Exception exception) =>
        exception is not (
            AccessViolationException     or
            ArgumentNullException        or
            IndexOutOfRangeException     or
            InvalidEnumArgumentException or
            NullReferenceException       or
            OutOfMemoryException         or
            StackOverflowException       or
            ThreadAbortException
        );
}
