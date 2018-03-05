using System.Collections.Generic;
using System.Linq;
using BlackjackSimulator.Extensions;
using BlackjackSimulator.Strategies.Interfaces;
using GamblingLibrary.Interfaces;

namespace BlackjackSimulator.Strategies
{
    public class StandardDealerStrategy : IDealerStrategy
    {
        public bool ShouldHit(List<ICard> currentHand)
        {
            var currentHandCardValues = currentHand.GetCardValues();
            return !MeetsStayCriteria(currentHandCardValues) && MeetsHitCriteria(currentHandCardValues);
        }

        private static bool MeetsHitCriteria(List<int> currentHandCardValues)
        {
            return currentHandCardValues.All(cv => cv < Constants.DealersMinimumTargetHandValue);
        }

        private static bool MeetsStayCriteria(List<int> currentHandCardValues)
        {
            return currentHandCardValues.Any(cv => cv >= Constants.DealersMinimumTargetHandValue && cv <= Constants.BestHandValue);
        }
    }
}