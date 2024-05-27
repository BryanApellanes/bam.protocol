/*
    Copyright © Bryan Apellanes 2015
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bam.ServiceProxy;
using Bam.Protocol;

namespace Bam.Server
{
    public interface IHttpResponder: IInitialize
    {
        string Name { get; }
        event ResponderEventHandler Responded;
        event ResponderEventHandler DidNotRespond;
        event ContentNotFoundEventHandler ContentNotFound;
        bool Respond(IHttpContext context);
        bool TryRespond(IHttpContext context);
    }
}