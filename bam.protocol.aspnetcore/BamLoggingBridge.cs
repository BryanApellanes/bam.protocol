using Microsoft.Extensions.Logging;

namespace Bam.Protocol.AspNetCore;

/// <summary>
/// Hands a component written against <see cref="ILogger{TCategoryName}"/> to Bam's own logging, for a host built on a
/// bamtk <c>ServiceRegistry</c> that has no <see cref="ILoggerFactory"/> (bam.protocol#59). Error and Critical entries
/// go to <see cref="Bam.Logging.ILog.Error(string, Exception?, object[])"/>, Warning to
/// <see cref="Bam.Logging.ILog.Warning"/>, Information to <see cref="Bam.Logging.ILog.Info"/>; Debug and Trace are
/// dropped. The already formatted message is passed as an argument, never as the message signature, so braces in it are
/// not read as placeholders, and its control characters are replaced first (<see cref="Neutralise"/>), because Bam's
/// sinks are line-oriented and a line break in a logged value would forge an entry.
/// </summary>
/// <remarks>
/// A stop-gap. bamgateway has the fuller bridge (<c>BamLoggerProvider</c>: a provider, scopes, Critical as Fatal), and
/// BryanApellanes/bam.logging#2 moves that one into bamtk. When it lands, this class and
/// <see cref="ActorAuthenticationRegistration.LoggerFrom{T}"/> are to be retired in its favour. Until then the two
/// neutralise control characters the same way.
/// </remarks>
/// <typeparam name="T">The category the entries are written under; it prefixes each message.</typeparam>
public sealed class BamLoggingBridge<T> : ILogger<T>
{
    private const string Signature = "{0}: {1}";
    private static readonly string Category = typeof(T).Name;

    /// <summary>Creates a bridge to a Bam log.</summary>
    /// <param name="target">The log to write to, or null to write to the process default, <see cref="Bam.Logging.Log"/>.</param>
    public BamLoggingBridge(Bam.Logging.ILog? target = null)
    {
        Target = target;
    }

    /// <summary>Gets the log entries are written to, or null when they go to the process default, <see cref="Bam.Logging.Log"/>.</summary>
    public Bam.Logging.ILog? Target { get; }

    /// <inheritdoc />
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return null;
    }

    /// <inheritdoc />
    public bool IsEnabled(LogLevel logLevel)
    {
        return logLevel >= LogLevel.Information && logLevel != LogLevel.None;
    }

    /// <inheritdoc />
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        ArgumentNullException.ThrowIfNull(formatter);
        if (!IsEnabled(logLevel))
        {
            return;
        }

        string message = Neutralise(formatter(state, exception) ?? string.Empty);
        // One path whether or not a target was given: the process default is an ILog like any other.
        Bam.Logging.ILog target = Target ?? Bam.Logging.Log.Default!;
        if (logLevel >= LogLevel.Error)
        {
            target.Error(Signature, exception, Category, message);
        }
        else if (logLevel == LogLevel.Warning)
        {
            target.Warning(Signature, Category, message);
        }
        else
        {
            target.Info(Signature, Category, message);
        }
    }

    /// <summary>
    /// Replaces control characters: CR and LF become the visible <c>\r</c> and <c>\n</c>, a tab becomes a space, any
    /// other becomes <c>?</c>. The same rule as bamgateway's <c>BamLoggerProvider.Neutralise</c>.
    /// </summary>
    /// <param name="value">The text.</param>
    /// <returns>The text with no control characters; the same instance when it had none.</returns>
    public static string Neutralise(string value)
    {
        ArgumentNullException.ThrowIfNull(value);
        System.Text.StringBuilder? clean = null;
        for (int index = 0; index < value.Length; index++)
        {
            char character = value[index];
            if (!char.IsControl(character))
            {
                clean?.Append(character);
                continue;
            }
            clean ??= new System.Text.StringBuilder(value.Length + 8).Append(value, 0, index);
            switch (character)
            {
                case '\r':
                    clean.Append("\\r");
                    break;
                case '\n':
                    clean.Append("\\n");
                    break;
                case '\t':
                    clean.Append(' ');
                    break;
                default:
                    clean.Append('?');
                    break;
            }
        }
        return clean?.ToString() ?? value;
    }
}
