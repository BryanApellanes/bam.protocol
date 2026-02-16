using System.Reflection;

namespace Bam.Protocol.Server;

/// <summary>
/// Subscribes a delegate to a named event on an event source using reflection.
/// </summary>
public class BamEventListener
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BamEventListener"/> class.
    /// </summary>
    public BamEventListener()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BamEventListener"/> class with the specified event name and handler.
    /// </summary>
    /// <param name="eventName">The name of the event to listen to.</param>
    /// <param name="eventHandler">The delegate to invoke when the event is raised.</param>
    public BamEventListener(string eventName, Delegate eventHandler)
    {
        this.EventName = eventName;
        this.EventHandler = eventHandler;
    }

    /// <summary>
    /// Gets or sets the name of the event to listen for.
    /// </summary>
    public string EventName { get; set; }

    /// <summary>
    /// Gets or sets the delegate to invoke when the event is raised.
    /// </summary>
    public Delegate EventHandler { get; set; }

    /// <summary>
    /// Subscribes the event handler to the named event on the specified event source.
    /// </summary>
    /// <param name="eventSource">The object that defines the event.</param>
    /// <param name="throwIfEventNotFound">If true, throws an exception when the event is not found on the source.</param>
    public void Listen(object eventSource, bool throwIfEventNotFound = false)
    {
        Type type = eventSource.GetType();
        EventInfo eventInfo = type.GetEvent(EventName);
        if (eventInfo != null)
        {
            eventInfo.AddEventHandler(eventSource, EventHandler);
            return;
        }

        if (throwIfEventNotFound)
        {
            throw new ArgumentException($"Specified event source does not define an event named {EventName}");
        }
    }
}