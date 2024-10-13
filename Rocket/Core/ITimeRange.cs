using System;

namespace Rocket.Core
{
    public interface ITimeRange
    {
        DateTime StartedAt{get;set;}
        DateTime EndedAt{get;set;}
    }
}