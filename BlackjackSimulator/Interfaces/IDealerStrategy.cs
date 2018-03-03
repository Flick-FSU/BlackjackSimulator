using System.Collections.Generic;
using GamblingLibrary.Interfaces;

namespace BlackjackSimulator.Interfaces
{
    public interface IDealerStrategy
    {
        bool ShouldHit(List<ICard> currentHand);
    }
}