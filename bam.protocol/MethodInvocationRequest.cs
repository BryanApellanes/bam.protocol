using System.Reflection;
using Bam.DependencyInjection;
using Newtonsoft.Json;

namespace Bam.Protocol;

public class MethodInvocationRequest : IInvocationRequest
{
    // For deserialization
    public MethodInvocationRequest()
    {
        this.Arguments = new List<Argument>();
    }
    // --
    
    // For client instantiation
    public MethodInvocationRequest(MethodInfo methodInfo): this(null, methodInfo)
    {
    }
    
    public MethodInvocationRequest(object instance, string operationIdentifier): this(instance, Protocol.OperationIdentifier.ToMethod(operationIdentifier))
    {
    }

    public MethodInvocationRequest(object instance, MethodInfo methodInfo)
    {
        Args.ThrowIfNull(methodInfo, nameof(methodInfo));
        this.Instance = instance;
        this.MethodInfo = methodInfo;
    }
    // --

    public MethodInvocationRequest(object instance, string methodName, params object[] arguments)
    {
        Args.ThrowIfNull(instance);
        Args.ThrowIfNull(methodName, nameof(methodName));
        this.Instance = instance;
        this.MethodInfo = instance.GetType().GetMethod(methodName, arguments.Select(arg => arg.GetType()).ToArray());
        Args.ThrowIfNull(this.MethodInfo, nameof(this.MethodInfo));
    }
    
    public static MethodInvocationRequest For(object instance, string methodName, params object[] arguments)
    {
        Args.ThrowIfNull(instance, nameof(instance));
        Type type = instance.GetType();
        MethodInfo methodInfo = type.GetMethod(methodName);
        Args.ThrowIfNull(methodInfo, nameof(methodInfo));

        string operationIdentifier = Protocol.OperationIdentifier.For(methodInfo);
        return new MethodInvocationRequest(instance, operationIdentifier)
        {
            Arguments = Argument.ListForValues(methodInfo, arguments)
        };
    }

    public static MethodInvocationRequest For<T>(string methodName, params object[] arguments)
    {
        return For(typeof(T), methodName, arguments);
    }
    
    public static MethodInvocationRequest For(Type type, string methodName, params object[] arguments)
    {
        return For(type.GetMethod(methodName), arguments);
    }
    
    public static MethodInvocationRequest For(MethodInfo methodInfo, params object[] arguments)
    {
        Args.ThrowIfNull(methodInfo, nameof(methodInfo));
        
        return new MethodInvocationRequest(methodInfo)
        {
            Arguments =  Argument.ListForValues(methodInfo, arguments)
        };
    }

    /// <summary>
    /// Initializes the invocation request for a client to transmit to a server.  Only necessary to capture instance context if required.
    /// </summary>
    /// <param name="instanceProvider"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public void ClientInitialize(ServiceRegistry instanceProvider = null)
    {
        if (MethodInfo != null)
        {
            this.OperationIdentifier = Bam.Protocol.OperationIdentifier.For(MethodInfo);


            if (!MethodInfo.IsStatic)
            {
                if (Instance == null)
                {
                    if (instanceProvider == null)
                    {
                        throw new InvalidOperationException("Instance is null and no ServiceRegistry provider was specified.");
                    }
                    Type? type = MethodInfo.DeclaringType;
                    if (type != null)
                    {
                        Instance = instanceProvider.Get(type);
                    }
                }

                if (Instance != null)
                {
                    InvocationContextSerializer serializer = instanceProvider.Get<InvocationContextSerializer>();
                    ContextSerializationFormat = serializer.Format;
                    SerializedContext = serializer.Serialize(Instance);
                }
            }
        }
    }

    public void ServerInitialize(ServiceRegistry instanceProvider = null)
    {
        if (MethodInfo == null)
        {
            SetMethodInfo(instanceProvider);
        }

        if (!MethodInfo.IsStatic && Instance == null)
        {
            if (string.IsNullOrEmpty(SerializedContext))
            {
                Instance = instanceProvider.Get(MethodInfo.DeclaringType);
            }
            else
            {
                Instance = instanceProvider.Get<InvocationContextSerializer>().Deserialize(MethodInfo.DeclaringType, SerializedContext);
            }
        }
    }
    
    protected object Instance { get; set; }
    protected MethodInfo MethodInfo { get; set; }
    

    public string ContextSerializationFormat { get; private set; }
    public string OperationIdentifier { get; set; }
    public string SerializedContext { get; set; }
    
    public List<Argument> Arguments { get; set; }

    public T Invoke<T>()
    {
        return (T)Invoke();
    }
    public object Invoke()
    {
        return MethodInfo.Invoke(Instance, Arguments.Select(a => a.Value).ToArray());
    }
    
    /// <summary>
    /// Resolves the method before server invocation.
    /// </summary>
    /// <param name="instanceProvider"></param>
    protected void SetMethodInfo(ServiceRegistry instanceProvider = null)
    {
        MethodInfo = Bam.Protocol.OperationIdentifier.ToMethod(OperationIdentifier);
        if (!MethodInfo.IsStatic)
        {
            if (SerializedContext == null)
            {
                Instance = instanceProvider.Get(MethodInfo.DeclaringType);
            }
            else
            {
                InvocationContextSerializer serializer = instanceProvider.Get<InvocationContextSerializer>();
                Instance = serializer.Deserialize(MethodInfo.DeclaringType, SerializedContext);
            }
        }
    }
}