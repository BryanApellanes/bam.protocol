using System.Globalization;
using System.Text.Json;
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
/// defaults say. Failure throws an <see cref="InvocationArgumentException"/> that
/// names the parameter and both types.
/// </remarks>
public static class InvocationArgumentCoercer
{
    /// <summary>
    /// The serializer every Newtonsoft conversion here goes through. It is pinned to no polymorphic type handling
    /// so a host's <see cref="JsonConvert.DefaultSettings"/> can never let a caller-supplied <c>$type</c> choose the
    /// type that gets instantiated.
    /// </summary>
    private static readonly Newtonsoft.Json.JsonSerializer SafeSerializer = Newtonsoft.Json.JsonSerializer.Create(new JsonSerializerSettings
    {
        TypeNameHandling = TypeNameHandling.None,
        MetadataPropertyHandling = MetadataPropertyHandling.Ignore
    });

    /// <summary>
    /// Coerces <paramref name="value"/> to <paramref name="targetType"/>.
    /// </summary>
    /// <param name="parameterName">The target parameter's name, used only in the failure message.</param>
    /// <param name="value">The deserialized argument value, possibly null.</param>
    /// <param name="targetType">The parameter type the method declares.</param>
    /// <returns>A value assignable to <paramref name="targetType"/>.</returns>
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

        if (targetType.IsInstanceOfType(value))
        {
            return value;
        }

        try
        {
            if (value is JToken token)
            {
                return token.ToObject(targetType, SafeSerializer);
            }

            if (value is JsonElement element)
            {
                return System.Text.Json.JsonSerializer.Deserialize(element.GetRawText(), targetType);
            }

            if (underlying.IsEnum)
            {
                return CoerceEnum(value, underlying);
            }

            if (underlying == typeof(string))
            {
                return Convert.ToString(value, CultureInfo.InvariantCulture);
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

    private static object CoerceEnum(object value, Type enumType)
    {
        if (value is string name)
        {
            return Enum.Parse(enumType, name, ignoreCase: true);
        }

        object numeric = Convert.ChangeType(value, Enum.GetUnderlyingType(enumType), CultureInfo.InvariantCulture);
        return Enum.ToObject(enumType, numeric);
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
