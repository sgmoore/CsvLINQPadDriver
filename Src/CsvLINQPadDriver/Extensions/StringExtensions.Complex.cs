using System;
using System.Globalization;
using System.Numerics;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace CsvLINQPadDriver.Extensions;

public static partial class StringExtensions
{
    public static Complex? ToComplex(this string? s, NumberStyles style = Styles.Float, IFormatProvider? provider = null)
    {
        if (string.IsNullOrEmpty(s))
        {
            return null;
        }

        try
        {
            return Parse(s, style, provider.ResolveFormatProvider());
        }
        catch (Exception e) when (e.CanBeHandled())
        {
            throw ConvertException.Create("Complex", s, e);
        }
    }

#if NETCOREAPP
    public static Complex? ToComplex(this ReadOnlySpan<char> s, NumberStyles style = Styles.Float, IFormatProvider? provider = null)
    {
        if (s.IsEmpty)
        {
            return null;
        }

        try
        {
            return Parse(s, style, provider.ResolveFormatProvider());
        }
        catch (Exception e) when (e.CanBeHandled())
        {
            throw ConvertException.Create("Complex", s, e);
        }
    }
#endif

    public static Complex? ToComplexSafe(this string? s, NumberStyles style = Styles.Float, IFormatProvider? provider = null) =>
        TryParse(s, style, provider.ResolveFormatProvider(), out var parsedValue) ? parsedValue : null;

#if NETCOREAPP
    public static Complex? ToComplexSafe(this ReadOnlySpan<char> s, NumberStyles style = Styles.Float, IFormatProvider? provider = null) =>
        TryParse(s, style, provider.ResolveFormatProvider(), out var parsedValue) ? parsedValue : null;
#endif

    // Fixed TryParse/Parse source: https://github.com/dotnet/runtime/blob/main/src/libraries/System.Runtime.Numerics/src/System/Numerics/Complex.cs
    private const NumberStyles InvalidNumberStyles = ~(
          NumberStyles.AllowLeadingWhite   | NumberStyles.AllowTrailingWhite
        | NumberStyles.AllowLeadingSign    | NumberStyles.AllowTrailingSign
        | NumberStyles.AllowParentheses    | NumberStyles.AllowDecimalPoint
        | NumberStyles.AllowThousands      | NumberStyles.AllowExponent
        | NumberStyles.AllowCurrencySymbol | NumberStyles.AllowHexSpecifier);

    private static bool TryParse(
#if NETCOREAPP
        ReadOnlySpan<char>
#else
        string?
#endif
            s,
        NumberStyles style, IFormatProvider? provider, out Complex result)
    {
#if !NETCOREAPP
        if (s is null)
        {
            result = default;
            return false;
        }
#endif

        ValidateParseStyleFloatingPoint(style);

        var openBracket = s.IndexOf('<');
        var semicolon = s.IndexOf(';');
        var closeBracket = s.IndexOf('>');

        if (s.Length < 5 || openBracket == -1 || semicolon == -1 || closeBracket == -1 || openBracket > semicolon || openBracket > closeBracket || semicolon > closeBracket)
        {
            // We need at least 5 characters for `<0;0>`
            // We also expect a to find an open bracket, a semicolon, and a closing bracket in that order

            result = default;
            return false;
        }

        if (openBracket != 0 && ((style & NumberStyles.AllowLeadingWhite) == 0 ||
            !
#if !NETCOREAPP
            string.IsNullOrWhiteSpace(
#endif
            s[..openBracket]
#if NETCOREAPP
            .IsWhiteSpace(
#endif
            )
        ))
        {
            // The opening bracket wasn't the first and we either didn't allow leading whitespace
            // or one of the leading characters wasn't whitespace at all.

            result = default;
            return false;
        }

        if (!double.TryParse(s.
#if NETCOREAPP
            Slice
#else
            Substring
#endif
            (openBracket + 1, semicolon - 1),
            style, provider, out var real))
        {
            result = default;
            return false;
        }

        if (char.IsWhiteSpace(s[semicolon + 1]))
        {
            // We allow a single whitespace after the semicolon regardless of style, this is so that
            // the output of `ToString` can be correctly parsed by default and values will roundtrip.
            semicolon += 1;
        }

        if (!double.TryParse(s.
#if NETCOREAPP
            Slice
#else
            Substring
#endif
            (semicolon + 1, closeBracket - semicolon - 1),
            style, provider, out var imaginary))
        {
            result = default;
            return false;
        }

        if (closeBracket != s.Length - 1 && ((style & NumberStyles.AllowTrailingWhite) == 0 ||
            !
#if !NETCOREAPP
            string.IsNullOrWhiteSpace(
#endif
            s[closeBracket..]
#if NETCOREAPP
            .IsWhiteSpace(
#endif
            )
        ))
        {
            // The closing bracket wasn't the last and we either didn't allow trailing whitespace
            // or one of the trailing characters wasn't whitespace at all.

            result = default;
            return false;
        }

        result = new Complex(real, imaginary);
        return true;

        static void ValidateParseStyleFloatingPoint(NumberStyles style)
        {
            // Check for undefined flags or hex number
            if ((style & (InvalidNumberStyles | NumberStyles.AllowHexSpecifier)) != 0)
            {
                if ((style & InvalidNumberStyles) != 0)
                {
                    throw new ArgumentException("An undefined NumberStyles value is being used.", nameof(style));
                }

                throw new ArgumentException("The number style AllowHexSpecifier is not supported on floating point data types.");
            }
        }
    }

    private static Complex Parse(
#if NETCOREAPP
        ReadOnlySpan<char>
#else
        string?
#endif
            s, NumberStyles style, IFormatProvider? provider) =>
        TryParse(s, style, provider, out var result)
            ? result
            : throw new OverflowException();
}
