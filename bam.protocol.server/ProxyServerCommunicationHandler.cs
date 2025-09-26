using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bam.Protocol.Server
{
    public class ProxyServerCommunicationHandler : CommunicationHandler
    {
        public ProxyServerCommunicationHandler(ITcpIPAddressProvider tcpIpAddressProvider, IUdpIPAddressProvider udpIpAddressProvider, IBamRequestReader requestReader, IBamServerContextProvider serverContextProvider, IBamResponseProvider responseProvider, IActorResolver actorResolver, IServerSessionManager serverSessionManager, IAuthorizationCalculator authorizationCalculator, IBamRequestProcessor requestProcessor, IObjectEncoderDecoder objectEncoderDecoder) : base(tcpIpAddressProvider, udpIpAddressProvider, requestReader, serverContextProvider, responseProvider, actorResolver, serverSessionManager, authorizationCalculator, requestProcessor, objectEncoderDecoder)
        {
        }
    }
}
