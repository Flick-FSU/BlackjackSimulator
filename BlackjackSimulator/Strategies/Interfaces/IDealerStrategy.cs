using System.Collections.Generic;
using GamblingLibrary.Interfaces;

namespace BlackjackSimulator.Strategies.Interfaces
{
    public interface IDealerStrategy
    {
        bool ShouldHit(List<ICard> currentHand);
    }
}