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
/// defaults say. Conversions are strict where silence would hide a caller error: a <c>string</c> parameter accepts
/// text, primitives and dates (dates in round-trip <c>"O"</c> form) but not objects or <see cref="DBNull"/>; an enum
/// parameter rejects values the enum does not define (flags enums excepted); an integral parameter rejects a number
/// with a fractional part. Failure throws an <see cref="InvocationArgumentException"/> that
/// names the parameter and both types.
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

        try
        {
            if (underlying.IsEnum)
            {
                return CoerceEnum(parameterName, Unwrap(value), underlying);
            }

            if (IsIntegral(underlying))
            {
                RejectFraction(parameterName, Unwrap(value), targetType);
            }

            if (value is JToken token)
            {
                return token.ToObject(targetType, SafeSerializer);
            }

            if (value is JsonElement element)
            {
                return element.Deserialize(targetType, ElementOptions);
            }

            if (underlying == typeof(string))
            {
                return CoerceToString(parameterName, value, targetType);
            }

            if (value is string text)
            {
                object? parsed = ParseText(text, underlying);
                if (parsed is not null)
                {
                    return parsed;
                }
            }

            if (value is IConvertible && typeof(IConvertible).IsAssignableFrom(underlying))
            {
                return Convert.ChangeType(value, underlying, CultureInfo.InvariantCulture);
            }

            return JToken.FromObject(value, SafeSerializer).ToObject(targetType, SafeSerializer);
        }
        catch (Exception ex) when (ex is not InvocationArgumentException)
        {
            throw new InvocationArgumentException(parameterName, value.GetType(), targetType, ex);
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

    /// <summary>Takes the CLR value out of a scalar token or element so the scalar rules see one shape.</summary>
    private static object Unwrap(object value)
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
                    return element.TryGetInt64(out long whole) ? whole : element.GetDouble();
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

    private static void RejectFraction(string parameterName, object value, Type targetType)
    {
        decimal number;
        switch (value)
        {
            case double d when !double.IsNaN(d) && !double.IsInfinity(d):
                number = (decimal)d;
                break;
            case float f when !float.IsNaN(f) && !float.IsInfinity(f):
                number = (decimal)f;
                break;
            case decimal m:
                number = m;
                break;
            default:
                return;
        }

        if (number != decimal.Truncate(number))
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
            default:
                throw new InvocationArgumentException(parameterName, value.GetType(), targetType);
        }
    }

    private static object? ParseText(string text, Type underlying)
    {
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
