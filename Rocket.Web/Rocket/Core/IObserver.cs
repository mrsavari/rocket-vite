using System;

namespace Rocket.Core
{
    public interface IObserver
    {
         ISubject Subject { get; set; }
    }
}