using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bam.Protocol;

/// <summary>
/// Converts a deserialized invocation argument into the type the target method parameter declares.
/// </summary>
/// <remarks>
/// A <see cref="MethodInvocationRequest"/> crosses the wire as JSON, and its <c>Arguments</c> dictionary is typed
/// <c>object?</c>, so a deserializer hands back whatever its untyped representation is: Newtonsoft yields
/// <see cref="JToken"/>s (a <see cref="JObject"/> for a complex argument, a <see cref="JValue"/> holding a
/// <see cref="long"/> for any integer), System.Text.Json yields <see cref="JsonElement"/>s. Reflection refuses
/// those where the method declares a POCO, an <see cref="int"/> or an enum, so every argument is coerced here
/// before <see cref="MethodInvocationRequest.Invoke()"/> calls the method. The rules, in order: a null stays null
/// (or becomes the default of a non-nullable value type); a value already assignable to the parameter is passed
/// through; JSON tokens are materialised with their own serializer; enums accept their name or number; the
/// common string-encoded BCL types (<see cref="Guid"/>, <see cref="DateTimeOffset"/>, <see cref="TimeSpan"/>,
/// <see cref="Uri"/>) are parsed; <see cref="IConvertible"/> primitives go through
/// <see cref="Convert.ChangeType(object, Type, IFormatProvider)"/> with the invariant culture; anything else is
/// round-tripped through JSON as a last resort. Every Newtonsoft conversion uses a serializer pinned to
/// <see cref="TypeNameHandling.None"/>, so a <c>$type</c> in the payload is inert whatever the process-wide
/// defaults say. A scalar token from either reader (a <see cref="JValue"/>, or a <see cref="JsonElement"/> holding a
/// string, number or boolean) is reduced to its CLR value first, so both readers follow one set of scalar rules.
/// Conversions are strict where silence would hide a caller error: a <c>string</c> parameter accepts text, primitives
/// and dates (dates in round-trip <c>"O"</c> form) but not objects or <see cref="DBNull"/>; an enum parameter rejects
/// values the enum does not define (flags enums excepted); a boolean is accepted only by a <c>bool</c> or
/// <c>string</c> parameter and a <c>bool</c> parameter accepts only a boolean or text; an enum or integral parameter
/// rejects a number with a fractional part; numeric text is parsed without group separators; a JSON number keeps
/// its digits for a <c>decimal</c>, enum or integral parameter, reaches a <c>double</c> or <c>float</c> parameter as
/// the text's own nearest value, and reaches a <c>string</c> parameter as written. These guarantees hold at the
/// <em>parameter</em> boundary. Members nested inside a POCO or array argument are bound by the serializers' own
/// rules: an undefined enum number is stored as is, and a date-shaped string that the envelope reader already turned
/// into a date is rendered in the serializer's default format rather than round-trip form until bam.protocol#67
/// stops the reader from parsing dates. Failure throws an <see cref="InvocationArgumentException"/> that names the
/// parameter and both types.
/// </remarks>
public static class InvocationArgumentCoercer
{
    /// <summary>
    /// The serializer every Newtonsoft conversion here goes through. It is pinned to no polymorphic type handling
    /// so a host's <see cref="JsonConvert.DefaultSettings"/> can never let a caller-supplied <c>$type</c> choose the
    /// type that gets instantiated, and to a nesting limit that does not depend on how the envelope was read.
    /// </summary>
    private static readonly Newtonsoft.Json.JsonSerializer SafeSerializer = Newtonsoft.Json.JsonSerializer.Create(new JsonSerializerSettings
    {
        TypeNameHandling = TypeNameHandling.None,
        MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
        MaxDepth = 64
    });

    /// <summary>
    /// The options a <see cref="JsonElement"/> argument is bound with, matching what the Newtonsoft path accepts:
    /// case-insensitive property names and enum members by name.
    /// </summary>
    private static readonly JsonSerializerOptions ElementOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() },
        MaxDepth = 64
    };

    /// <summary>
    /// Coerces <paramref name="value"/> to <paramref name="targetType"/>.
    /// </summary>
    /// <param name="parameterName">The target parameter's name, used only in the failure message.</param>
    /// <param name="value">The deserialized argument value, possibly null.</param>
    /// <param name="targetType">The parameter type the method declares.</param>
    /// <returns>
    /// A value assignable to <paramref name="targetType"/>. The return type is <c>object?</c> on purpose: the only
    /// consumer is the <c>object?[]</c> that <see cref="System.Reflection.MethodBase.Invoke(object, object[])"/> takes.
    /// </returns>
    /// <exception cref="InvocationArgumentException">The value cannot be represented as <paramref name="targetType"/>.</exception>
    public static object? Coerce(string parameterName, object? value, Type targetType)
    {
        Args.ThrowIfNull(parameterName, nameof(parameterName));
        Args.ThrowIfNull(targetType, nameof(targetType));

        Type underlying = Nullable.GetUnderlyingType(targetType) ?? targetType;

        if (value is null || value is JValue { Type: JTokenType.Null } || value is JsonElement { ValueKind: JsonValueKind.Null })
        {
            return DefaultFor(targetType);
        }

        if (value is DBNull)
        {
            throw new InvocationArgumentException(parameterName, typeof(DBNull), targetType);
        }

        if (targetType.IsInstanceOfType(value))
        {
            return value;
        }

        // A scalar token from either reader becomes its CLR value here, so the rules below see one shape.
        object scalar = Unwrap(value, underlying);
        try
        {
            if (targetType.IsInstanceOfType(scalar))
            {
                return scalar;
            }

            // A boolean never becomes a number or an enum, and a number never becomes a boolean; only text may cross.
            bool crossesBoolean = scalar is bool
                ? underlying != typeof(bool) && underlying != typeof(string)
                : underlying == typeof(bool) && scalar is not string;
            if (crossesBoolean)
            {
                throw new InvocationArgumentException(parameterName, scalar.GetType(), targetType);
            }

            if (underlying.IsEnum)
            {
                RejectFraction(parameterName, scalar, targetType);
                return CoerceEnum(parameterName, scalar, underlying);
            }

            if (IsIntegral(underlying))
            {
                RejectFraction(parameterName, scalar, targetType);
            }

            if (scalar is JToken token)
            {
                return token.ToObject(targetType, SafeSerializer);
            }

            if (scalar is JsonElement element)
            {
                return element.Deserialize(targetType, ElementOptions);
            }

            if (underlying == typeof(string))
            {
                return CoerceToString(parameterName, scalar, targetType);
            }

            if (scalar is string text)
            {
                object? parsed = ParseText(parameterName, text, underlying, targetType);
                if (parsed is not null)
                {
                    return parsed;
                }
            }

            if (scalar is IConvertible && typeof(IConvertible).IsAssignableFrom(underlying))
            {
                return Convert.ChangeType(scalar, underlying, CultureInfo.InvariantCulture);
            }

            return JToken.FromObject(scalar, SafeSerializer).ToObject(targetType, SafeSerializer);
        }
        catch (Exception ex) when (ex is not InvocationArgumentException)
        {
            throw new InvocationArgumentException(parameterName, scalar.GetType(), targetType, ex);
        }
    }

    private static object? DefaultFor(Type targetType)
    {
        if (targetType.IsValueType && Nullable.GetUnderlyingType(targetType) is null)
        {
            return Activator.CreateInstance(targetType);
        }

        return null;
    }

    /// <summary>
    /// Takes the CLR value out of a scalar token or element so the scalar rules see one shape; objects, arrays and
    /// anything that is not a token come back unchanged. A JSON number needs the target to pick its CLR shape: no
    /// single numeric type keeps every digit for every target.
    /// </summary>
    private static object Unwrap(object value, Type underlying)
    {
        if (value is JValue scalar && scalar.Value is not null)
        {
            return scalar.Value;
        }

        if (value is JsonElement element)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.String:
                    return element.GetString()!;
                case JsonValueKind.Number:
                    if (underlying == typeof(string))
                    {
                        // A string parameter gets the number as it was written; no CLR numeric type keeps every spelling.
                        return element.GetRawText();
                    }

                    // Separate statements on purpose: a conditional expression would widen the long to double before boxing.
                    if (element.TryGetInt64(out long whole))
                    {
                        return whole;
                    }

                    // decimal keeps the raw digits (and the whole ulong range) for the targets whose digits it protects;
                    // every other target takes the double the raw text produces, because (double)decimal is not correctly
                    // rounded. A magnitude below decimal's 1e-28 floor reads as 0m: that zero is kept only when the double
                    // is zero too, so a tiny value falls to double and is refused for an integral or enum target.
                    bool keepsDigits = underlying == typeof(decimal) || underlying.IsEnum || IsIntegral(underlying);
                    if (keepsDigits && element.TryGetDecimal(out decimal exact) && (exact != 0m || element.GetDouble() == 0d))
                    {
                        return exact;
                    }

                    return element.GetDouble();
                case JsonValueKind.True:
                case JsonValueKind.False:
                    return element.GetBoolean();
            }
        }

        return value;
    }

    private static bool IsIntegral(Type type)
    {
        return type == typeof(int) || type == typeof(long) || type == typeof(short) || type == typeof(byte)
            || type == typeof(uint) || type == typeof(ulong) || type == typeof(ushort) || type == typeof(sbyte);
    }

    /// <summary>
    /// Refuses a non-finite number and a number with a fractional part for an integral or enum target. The test is
    /// made on the value's own type (no conversion to <see cref="decimal"/>, which would round to fifteen significant
    /// digits first). Booleans are refused before this runs.
    /// </summary>
    private static void RejectFraction(string parameterName, object value, Type targetType)
    {
        bool whole;
        switch (value)
        {
            case double d:
                whole = double.IsFinite(d) && Math.Floor(d) == d;
                break;
            case float f:
                whole = float.IsFinite(f) && MathF.Floor(f) == f;
                break;
            case decimal m:
                whole = decimal.Truncate(m) == m;
                break;
            default:
                return;
        }

        if (!whole)
        {
            throw new InvocationArgumentException(parameterName, value.GetType(), targetType);
        }
    }

    private static object CoerceEnum(string parameterName, object value, Type enumType)
    {
        object result;
        if (value is string name)
        {
            result = Enum.Parse(enumType, name, ignoreCase: true);
        }
        else
        {
            object numeric = Convert.ChangeType(value, Enum.GetUnderlyingType(enumType), CultureInfo.InvariantCulture);
            result = Enum.ToObject(enumType, numeric);
        }

        bool flags = enumType.IsDefined(typeof(FlagsAttribute), inherit: false);
        if (!flags && !Enum.IsDefined(enumType, result))
        {
            throw new InvocationArgumentException(parameterName, value.GetType(), enumType);
        }

        return result;
    }

    private static string CoerceToString(string parameterName, object value, Type targetType)
    {
        switch (value)
        {
            case string text:
                return text;
            case DateTime dateTime:
                return dateTime.ToString("O", CultureInfo.InvariantCulture);
            case DateTimeOffset dateTimeOffset:
                return dateTimeOffset.ToString("O", CultureInfo.InvariantCulture);
            case Guid guid:
                return guid.ToString("D");
            case TimeSpan span:
                return span.ToString("c", CultureInfo.InvariantCulture);
            case Uri uri:
                return uri.OriginalString;
            case bool or char or sbyte or byte or short or ushort or int or uint or long or ulong or float or double or decimal:
                return Convert.ToString(value, CultureInfo.InvariantCulture)!;
            case System.Numerics.BigInteger bigInteger:
                return bigInteger.ToString(CultureInfo.InvariantCulture);
            default:
                throw new InvocationArgumentException(parameterName, value.GetType(), targetType);
        }
    }

    /// <summary>
    /// Parses the text forms of the BCL types that <see cref="Convert.ChangeType(object, Type, IFormatProvider)"/>
    /// does not handle, and numbers without group separators (<c>"1,5"</c> is refused rather than read as 15).
    /// </summary>
    private static readonly char[] ExponentMarkers = { 'e', 'E' };

    private static object? ParseText(string parameterName, string text, Type underlying, Type targetType)
    {
        if (IsIntegral(underlying))
        {
            decimal number = decimal.Parse(text, NumberStyles.Float, CultureInfo.InvariantCulture);
            int exponentAt = text.IndexOfAny(ExponentMarkers);
            ReadOnlySpan<char> mantissa = exponentAt < 0 ? text.AsSpan() : text.AsSpan(0, exponentAt);
            if (number == 0m && mantissa.IndexOfAnyInRange('1', '9') >= 0)
            {
                // "1e-30" parses to zero because decimal rounds past 28 places; a non-zero mantissa that reads as zero was rounded away ("0e5" is a real zero).
                throw new InvocationArgumentException(parameterName, typeof(string), targetType);
            }

            RejectFraction(parameterName, number, targetType);
            return Convert.ChangeType(number, underlying, CultureInfo.InvariantCulture);
        }

        if (underlying == typeof(double))
        {
            return double.Parse(text, NumberStyles.Float, CultureInfo.InvariantCulture);
        }

        if (underlying == typeof(float))
        {
            return float.Parse(text, NumberStyles.Float, CultureInfo.InvariantCulture);
        }

        if (underlying == typeof(decimal))
        {
            return decimal.Parse(text, NumberStyles.Float, CultureInfo.InvariantCulture);
        }

        if (underlying == typeof(Guid))
        {
            return Guid.Parse(text);
        }

        if (underlying == typeof(DateTimeOffset))
        {
            return DateTimeOffset.Parse(text, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
        }

        if (underlying == typeof(DateTime))
        {
            return DateTime.Parse(text, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
        }

        if (underlying == typeof(TimeSpan))
        {
            return TimeSpan.Parse(text, CultureInfo.InvariantCulture);
        }

        if (underlying == typeof(Uri))
        {
            return new Uri(text, UriKind.RelativeOrAbsolute);
        }

        return null;
    }
}
