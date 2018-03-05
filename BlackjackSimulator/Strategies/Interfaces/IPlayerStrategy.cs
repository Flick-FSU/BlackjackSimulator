using BlackjackSimulator.Entities.Interfaces;
using BlackjackSimulator.Models;
using GamblingLibrary.Interfaces;

namespace BlackjackSimulator.Strategies.Interfaces
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