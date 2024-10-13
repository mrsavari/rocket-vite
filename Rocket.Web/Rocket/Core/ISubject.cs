using System;

namespace Rocket.Core
{
    public interface ISubject
    {
        void Notify();
        event EventHandler OnNotify;
    }
}