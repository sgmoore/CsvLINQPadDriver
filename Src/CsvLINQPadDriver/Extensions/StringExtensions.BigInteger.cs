using System;
using System.Globalization;
using System.Numerics;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace CsvLINQPadDriver.Extensions;

public static partial class StringExtensions
{
    public static BigInteger? ToBigInteger(this string? s, NumberStyles style = Styles.Integer, IFormatProvider? provider = null)
    {
        if (string.IsNullOrEmpty(s))
        {
            return null;
        }

        try
        {
            return BigInteger.Parse(s, style, provider.ResolveFormatProvider());
        }
        catch (Exception e) when (e.CanBeHandled())
        {
            throw ConvertException.Create("BigInteger", s, e);
        }
    }

#if NETCOREAPP
    public static BigInteger? ToBigInteger(this ReadOnlySpan<char> s, NumberStyles style = Styles.Integer, IFormatProvider? provider = null)
    {
        if (s.IsEmpty)
        {
            return null;
        }

        try
        {
            return BigInteger.Parse(s, style, provider.ResolveFormatProvider());
        }
        catch (Exception e) when (e.CanBeHandled())
        {
            throw ConvertException.Create("BigInteger", s, e);
        }
    }
#endif

    public static BigInteger? ToBigIntegerSafe(this string? s, NumberStyles style = Styles.Integer, IFormatProvider? provider = null) =>
        BigInteger.TryParse(s, style, provider.ResolveFormatProvider(), out var parsedValue) ? parsedValue : null;

#if NETCOREAPP
    public static BigInteger? ToBigIntegerSafe(this ReadOnlySpan<char> s, NumberStyles style = Styles.Integer, IFormatProvider? provider = null) =>
        BigInteger.TryParse(s, style, provider.ResolveFormatProvider(), out var parsedValue) ? parsedValue : null;
#endif
}
