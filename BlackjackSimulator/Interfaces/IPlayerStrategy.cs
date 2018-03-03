using BlackjackSimulator.Models;
using GamblingLibrary.Interfaces;

namespace BlackjackSimulator.Interfaces
{
    public interface IPlayerStrategy
    {
        decimal GetInitialBetAmount(IPlayerHand lastHand, decimal playerTotalCash, TableSettings tableSettings);
        bool ShouldDoubleDown(IPlayerHand currentHand, ICard visibleCard);
        bool ShouldSplit(IPlayerHand currentHand, ICard visibleCard);
        bool ShouldHit(IPlayerHand currentHand, ICard visibleCard);
        bool ShouldLeaveTable(decimal playerTotalCash, TableSettings tableSettings);
    }
}