using System;
using System.Runtime.CompilerServices;
#if !NET8_0_OR_GREATER
using System.Runtime.Serialization;
#endif

namespace CsvLINQPadDriver;

[Serializable]
public class ConvertException : Exception
{
    private ConvertException(string type, string? value, Exception? innerException)
        : base($@"Failed to convert ""{value}"" to {type}.", innerException)
    {
    }

#if !NET8_0_OR_GREATER
    protected ConvertException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
#endif

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static ConvertException Create(string type, string? value, Exception? innerException) =>
        new(type, value, innerException);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static ConvertException Create(string type, ReadOnlySpan<char> value, Exception? innerException) =>
        new(type, value.ToString(), innerException);
}
