/*
    Copyright © Bryan Apellanes 2015
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bam.ServiceProxy;

namespace Bam.Server
{
    /// <summary>
    /// The delegate used to define responder events
    /// </summary>
    /// <param name="responder"></param>
    /// <param name="context"></param>
    public delegate void ResponderEventHandler(IHttpResponder responder, IHttpContext context);
}