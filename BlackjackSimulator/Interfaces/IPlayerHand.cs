using System.Collections.Generic;
using BlackjackSimulator.Enums;
using GamblingLibrary.Interfaces;

namespace BlackjackSimulator.Interfaces
{
    public interface IPlayerHand
    {
        decimal Bet { get; set; }
        List<ICard> Cards { get; }
        HandOutcome Outcome { get; set; }
        bool IsBlackjack { get; }
        decimal TotalPlayerCashAfterOutcome { get; set; }
        bool IsASplit { get; }
        bool IsADoubleDown { get; set; }

        bool CanSplit();
        List<int> GetCardValues();
        int GetBestCardValue();
        IPlayerHand Split();
        IPlayerHand GetDeepCopy();
    }
}