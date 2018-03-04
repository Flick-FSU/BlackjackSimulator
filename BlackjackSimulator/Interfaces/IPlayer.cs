using System.Collections.Generic;
using GamblingLibrary.Interfaces;

namespace BlackjackSimulator.Interfaces
{
    public interface IPlayer
    {
        List<IPlayerHand> CurrentHands { get; }
        List<IPlayerHand> HandHistory { get; }
        decimal CurrentTotalCash { get; set; }
        decimal StartingCash { get; set; }
        bool DoesNeedCard { get; }
        IPlayerHand InPlayHand { get; }
        bool IsAtTable { get; }
        string StrategyName { get; }

        void TakeCard(ICard card);
        void PlaceInitialBet();
        void PlayTurn(ICard visibleCard);
        void JoinTableWith(IDealer dealer);
        void LeaveTableOrStay();
        void SaveCurrentHands();
        void ClearCurrentHands();
    }
}