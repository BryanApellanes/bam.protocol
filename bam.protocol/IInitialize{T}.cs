using System;

namespace Bam.Protocol;

public interface IInitialize<T>: IInitialize
{
    event Action<T> Initializing;
    event Action<T> Initialized;
}