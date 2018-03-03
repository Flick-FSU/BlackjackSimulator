using System;
using System.Collections.Generic;
using System.Linq;
using BlackjackSimulator.Interfaces;
using BlackjackSimulator.Models;
using GamblingLibrary.Enums;
using GamblingLibrary.Interfaces;

namespace BlackjackSimulator.Strategies
{
    public class BasicMinimumPlayerStrategy : IPlayerStrategy
    {
        private const int DOUBLE_DOWN_VALUE = 11;

        public decimal GetInitialBetAmount(IPlayerHand lastHand, decimal playerTotalCash, TableSettings tableSettings)
        {
            if (playerTotalCash < tableSettings.MinimumBet)
                throw new InvalidOperationException("Not enough money to play the game");

            return tableSettings.MinimumBet;
        }

        public bool ShouldDoubleDown(IPlayerHand currentHand, ICard visibleCard)
        {
            return currentHand.GetCardValues().Contains(DOUBLE_DOWN_VALUE) &&
                   !currentHand.GetCardValues().Contains(Constants.BestHandValue);
        }

        public bool ShouldSplit(IPlayerHand currentHand, ICard visibleCard)
        {
            return currentHand.Cards.Count == 2 && currentHand.Cards[0].Type == CardType.Ace &&
                   currentHand.Cards[1].Type == CardType.Ace;
        }

        public bool ShouldHit(IPlayerHand currentHand, ICard visibleCard)
        {
            var currentHandCardValues = currentHand.GetCardValues();
            return !MeetsStayCriteria(currentHandCardValues) && MeetsHitCriteria(currentHandCardValues);
        }

        public bool ShouldLeaveTable(decimal playerTotalCash, TableSettings tableSettings)
        {
            return playerTotalCash < tableSettings.MinimumBet;
        }

        private static bool MeetsHitCriteria(List<int> currentHandCardValues)
        {
            return currentHandCardValues.Any(cv => cv < Constants.DealersMinimumTargetHandValue);
        }

        private static bool MeetsStayCriteria(List<int> currentHandCardValues)
        {
            return currentHandCardValues.Any(cv => cv >= Constants.DealersMinimumTargetHandValue && cv <= Constants.BestHandValue);
        }
    }
}