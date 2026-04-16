using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bam.Protocol.Server
{
    /// <summary>
    /// Communication handler for proxy server operations, extending the default <see cref="CommunicationHandler"/>.
    /// </summary>
    public class ProxyServerCommunicationHandler : CommunicationHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProxyServerCommunicationHandler"/> class with the specified communication components.
        /// </summary>
        /// <param name="tcpIpAddressProvider">The TCP IP address provider.</param>
        /// <param name="udpIpAddressProvider">The UDP IP address provider.</param>
        /// <param name="requestReader">The request reader.</param>
        /// <param name="serverContextProvider">The server context provider.</param>
        /// <param name="responseProvider">The response provider.</param>
        /// <param name="actorResolver">The actor resolver.</param>
        /// <param name="serverSessionManager">The session manager.</param>
        /// <param name="authorizationCalculator">The authorization calculator.</param>
        /// <param name="requestProcessor">The request processor.</param>
        /// <param name="objectEncoderDecoder">The object encoder/decoder.</param>
        public ProxyServerCommunicationHandler(ITcpIPAddressProvider tcpIpAddressProvider, IUdpIPAddressProvider udpIpAddressProvider, IBamRequestReader requestReader, IBamServerContextProvider serverContextProvider, IBamResponseProvider responseProvider, IActorResolver actorResolver, IServerSessionManager serverSessionManager, IAuthorizationCalculator authorizationCalculator, IBamRequestProcessor requestProcessor, IObjectEncoderDecoder objectEncoderDecoder) : base(tcpIpAddressProvider, udpIpAddressProvider, requestReader, serverContextProvider, responseProvider, actorResolver, serverSessionManager, authorizationCalculator, requestProcessor, objectEncoderDecoder)
        {
        }
    }
}
