using System;
using System.Collections.Generic;
using BlackjackSimulator.Entities.Interfaces;
using BlackjackSimulator.Extensions;
using BlackjackSimulator.Models;
using BlackjackSimulator.Strategies.Interfaces;
using GamblingLibrary.Interfaces;

namespace BlackjackSimulator.Entities
{
    public class Dealer : IDealer
    {
        public List<IPlayer> RegisteredPlayers { get; }
        public List<ICard> CurrentCards { get; }
        public ICard VisibleCard => CurrentCards[1];
        public TableSettings TableSettings { get; private set; }

        private readonly IGroupOfCards _cardDeck;
        private readonly IDealerStrategy _dealerStrategy;
        private readonly IGameManager _gameManager;

        public Dealer(IGroupOfCards cardDeck, IGameManager gameManager, IDealerStrategy dealerStrategy)
        {
            _cardDeck = cardDeck;
            _gameManager = gameManager;
            _dealerStrategy = dealerStrategy;
            RegisteredPlayers = new List<IPlayer>();
            CurrentCards = new List<ICard>();
        }

        public void SetTableSettingsWith(TableSettings tableSettings)
        {
            TableSettings = tableSettings;
        }

        public void Register(IPlayer player)
        {
            RegisteredPlayers.Add(player);
        }

        public void Unregister(IPlayer player)
        {
            RegisteredPlayers.Remove(player);
        }

        //todo: template method pattern
        public void PlaySingleGame()
        {
            if (RegisteredPlayers.Count == 0)
                throw new InvalidOperationException("Cannot start a game with no players");

            _cardDeck.Shuffle();
            _gameManager.PlaceYourBets(RegisteredPlayers);
            _gameManager.DealInitialCards(_cardDeck, RegisteredPlayers, CurrentCards);
            if (!CurrentCards.IsBlackjack())
            {
                _gameManager.PlayersPlay(_cardDeck, RegisteredPlayers, VisibleCard);
                _gameManager.DealerPlays(_cardDeck, CurrentCards, _dealerStrategy);
            }
            _gameManager.DeterminePlayerHandOutcomes(CurrentCards, RegisteredPlayers);
            _gameManager.PayoutOrCollectBetsFrom(RegisteredPlayers);
            _gameManager.SaveCurrentHandResultsOf(RegisteredPlayers);
            _cardDeck.Cards.AddRange(_gameManager.CollectCardsFrom(RegisteredPlayers));
            _cardDeck.Cards.AddRange(_gameManager.CollectCardsFrom(CurrentCards));
            _gameManager.ClearHandsFrom(RegisteredPlayers);
            _gameManager.DetermineToLeaveGameOrStay(RegisteredPlayers);
        }
    }
}