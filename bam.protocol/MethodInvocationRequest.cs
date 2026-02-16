using System.Reflection;
using Bam.DependencyInjection;
using Newtonsoft.Json;

namespace Bam.Protocol;

/// <summary>
/// Represents a request to invoke a specific method, including operation identifier, serialized context, and arguments.
/// </summary>
public class MethodInvocationRequest : IInvocationRequest
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MethodInvocationRequest"/> class for deserialization.
    /// </summary>
    public MethodInvocationRequest()
    {
        this.Arguments = new List<Argument>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MethodInvocationRequest"/> class for the specified method.
    /// </summary>
    /// <param name="methodInfo">The method to invoke.</param>
    public MethodInvocationRequest(MethodInfo methodInfo): this(null, methodInfo)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MethodInvocationRequest"/> class with the specified instance and operation identifier.
    /// </summary>
    /// <param name="instance">The object instance to invoke the method on.</param>
    /// <param name="operationIdentifier">The operation identifier string.</param>
    public MethodInvocationRequest(object instance, string operationIdentifier): this(instance, Protocol.OperationIdentifier.ToMethod(operationIdentifier))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MethodInvocationRequest"/> class with the specified instance and method info.
    /// </summary>
    /// <param name="instance">The object instance to invoke the method on.</param>
    /// <param name="methodInfo">The method to invoke.</param>
    public MethodInvocationRequest(object instance, MethodInfo methodInfo)
    {
        Args.ThrowIfNull(methodInfo, nameof(methodInfo));
        this.Instance = instance;
        this.MethodInfo = methodInfo;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MethodInvocationRequest"/> class with the specified instance, method name, and arguments.
    /// </summary>
    /// <param name="instance">The object instance to invoke the method on.</param>
    /// <param name="methodName">The name of the method to invoke.</param>
    /// <param name="arguments">The arguments to pass to the method.</param>
    public MethodInvocationRequest(object instance, string methodName, params object[] arguments)
    {
        Args.ThrowIfNull(instance);
        Args.ThrowIfNull(methodName, nameof(methodName));
        this.Instance = instance;
        this.MethodInfo = instance.GetType().GetMethod(methodName, arguments.Select(arg => arg.GetType()).ToArray());
        Args.ThrowIfNull(this.MethodInfo, nameof(this.MethodInfo));
    }
    
    /// <summary>
    /// Creates a <see cref="MethodInvocationRequest"/> for the specified instance, method name, and arguments.
    /// </summary>
    /// <param name="instance">The object instance to invoke the method on.</param>
    /// <param name="methodName">The name of the method to invoke.</param>
    /// <param name="arguments">The arguments to pass to the method.</param>
    /// <returns>A new <see cref="MethodInvocationRequest"/> instance.</returns>
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

    /// <summary>
    /// Creates a <see cref="MethodInvocationRequest"/> for a method on the specified type.
    /// </summary>
    /// <typeparam name="T">The type containing the method.</typeparam>
    /// <param name="methodName">The name of the method to invoke.</param>
    /// <param name="arguments">The arguments to pass to the method.</param>
    /// <returns>A new <see cref="MethodInvocationRequest"/> instance.</returns>
    public static MethodInvocationRequest For<T>(string methodName, params object[] arguments)
    {
        return For(typeof(T), methodName, arguments);
    }

    /// <summary>
    /// Creates a <see cref="MethodInvocationRequest"/> for a method on the specified type.
    /// </summary>
    /// <param name="type">The type containing the method.</param>
    /// <param name="methodName">The name of the method to invoke.</param>
    /// <param name="arguments">The arguments to pass to the method.</param>
    /// <returns>A new <see cref="MethodInvocationRequest"/> instance.</returns>
    public static MethodInvocationRequest For(Type type, string methodName, params object[] arguments)
    {
        return For(type.GetMethod(methodName), arguments);
    }

    /// <summary>
    /// Creates a <see cref="MethodInvocationRequest"/> for the specified method info and arguments.
    /// </summary>
    /// <param name="methodInfo">The method to invoke.</param>
    /// <param name="arguments">The arguments to pass to the method.</param>
    /// <returns>A new <see cref="MethodInvocationRequest"/> instance.</returns>
    public static MethodInvocationRequest For(MethodInfo methodInfo, params object[] arguments)
    {
        Args.ThrowIfNull(methodInfo, nameof(methodInfo));
        
        return new MethodInvocationRequest(methodInfo)
        {
            Arguments =  Argument.ListForValues(methodInfo, arguments)
        };
    }

    /// <summary>
    /// Initializes the invocation request for a client to transmit to a server. Only necessary to capture instance context if required.
    /// </summary>
    /// <param name="instanceProvider">The optional service registry used to resolve instances for non-static methods.</param>
    /// <exception cref="InvalidOperationException">Thrown when the instance is null and no service registry is provided for a non-static method.</exception>
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

    /// <summary>
    /// Initializes the invocation request for server-side execution, resolving the method and instance context.
    /// </summary>
    /// <param name="instanceProvider">The service registry to resolve instances from.</param>
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
    

    /// <inheritdoc />
    public string ContextSerializationFormat { get; private set; }

    /// <inheritdoc />
    public string OperationIdentifier { get; set; }

    /// <inheritdoc />
    public string SerializedContext { get; set; }

    /// <inheritdoc />
    public List<Argument> Arguments { get; set; }

    /// <summary>
    /// Invokes the method and returns the result cast to the specified type.
    /// </summary>
    /// <typeparam name="T">The expected return type.</typeparam>
    /// <returns>The result of the method invocation.</returns>
    public T Invoke<T>()
    {
        return (T)Invoke();
    }

    /// <summary>
    /// Invokes the method with the stored arguments and returns the result.
    /// </summary>
    /// <returns>The result of the method invocation.</returns>
    public object Invoke()
    {
        return MethodInfo.Invoke(Instance, Arguments.Select(a => a.Value).ToArray());
    }
    
    /// <summary>
    /// Resolves the method and instance context before server-side invocation.
    /// </summary>
    /// <param name="instanceProvider">The optional service registry used to resolve instances and deserializers.</param>
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