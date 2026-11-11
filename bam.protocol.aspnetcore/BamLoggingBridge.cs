using Microsoft.Extensions.Logging;

namespace Bam.Protocol.AspNetCore;

/// <summary>
/// Hands a component written against <see cref="ILogger{TCategoryName}"/> to Bam's own logging, for a host built on a
/// bamtk <c>ServiceRegistry</c> that has no <see cref="ILoggerFactory"/> (bam.protocol#59). Error and Critical entries
/// go to <see cref="Bam.Logging.ILog.Error(string, Exception?, object[])"/>, Warning to
/// <see cref="Bam.Logging.ILog.Warning"/>, Information to <see cref="Bam.Logging.ILog.Info"/>; Debug and Trace are
/// dropped. The already formatted message is passed as an argument, never as the message signature, so braces in it are
/// not read as placeholders.
/// </summary>
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

        string message = formatter(state, exception);
        if (logLevel >= LogLevel.Error)
        {
            if (Target is not null)
            {
                Target.Error(Signature, exception, Category, message);
            }
            else if (exception is not null)
            {
                Bam.Logging.Log.Error(Signature, exception, Category, message);
            }
            else
            {
                Bam.Logging.Log.Error(Signature, Category, message);
            }
        }
        else if (logLevel == LogLevel.Warning)
        {
            if (Target is not null)
            {
                Target.Warning(Signature, Category, message);
            }
            else
            {
                Bam.Logging.Log.Warn(Signature, Category, message);
            }
        }
        else if (Target is not null)
        {
            Target.Info(Signature, Category, message);
        }
        else
        {
            Bam.Logging.Log.Info(Signature, Category, message);
        }
    }
}
