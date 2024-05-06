using System;

namespace Bam.Protocol.Server;

public class BamSessionStateProvider : IBamSessionStateProvider
{
    public IBamSessionState GetSession(IBamServerContext serverContext)
    {
        throw new NotImplementedException();
    }
}