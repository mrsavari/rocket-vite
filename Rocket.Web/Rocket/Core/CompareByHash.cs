using System;
using System.Collections;
using System.Collections.Generic;

namespace Rocket.Core
{
    public class CompareByHash<T> : IComparer<Frame<T>>
    {
        public int Compare(Frame<T> x, Frame<T> y)
        {
            if (x.Hash.ToString() == y.Hash.ToString())
                return 0;
            if (long.Parse(x.Hash.ToString()) < long.Parse(y.Hash.ToString()))
                return -1;
            return 1;
        }
    }
}