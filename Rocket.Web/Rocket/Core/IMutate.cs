using System;
using System.Collections.Generic;

namespace Rocket.Core
{
    public interface IMutate<T,U> : IEnumerable<T>
    {
        U Excute(IEnumerable<T> source);
    }
}