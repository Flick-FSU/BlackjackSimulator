using System.Collections.Generic;
using GamblingLibrary.Interfaces;

namespace BlackjackSimulator.Interfaces
{
    public interface IGameManager
    {
        void PlaceYourBets(List<IPlayer> players);
        void DealInitialCards(IGroupOfCards cardDeck, List<IPlayer> players, List<ICard> dealerCards);
        void PlayersPlay(IGroupOfCards cardDeck, List<IPlayer> players, ICard dealersVisibleCard);
        void DealerPlays(IGroupOfCards cardDeck, List<ICard> dealerCards, IDealerStrategy dealerStrategy);
        void DeterminePlayerHandOutcomes(List<ICard> dealerCards, List<IPlayer> players);
        decimal PayoutOrCollectBetsFrom(List<IPlayer> players);
        List<ICard> CollectCardsFrom(List<IPlayer> players);
        List<ICard> CollectCardsFrom(List<ICard> cards);
        void SaveCurrentHandResultsOf(List<IPlayer> players);
        void ClearHandsFrom(List<IPlayer> players);
        void DetermineToLeaveGameOrStay(List<IPlayer> players);
    }
}