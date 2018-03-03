using System;
using System.Collections.Generic;
using BlackjackSimulator.Enums;
using BlackjackSimulator.Extensions;
using BlackjackSimulator.Interfaces;
using GamblingLibrary;
using GamblingLibrary.Interfaces;

namespace BlackjackSimulator
{
    public class GameManager : IGameManager
    {
        public void PlaceYourBets(List<IPlayer> players)
        {
            foreach (var blackjackPlayer in players)
                blackjackPlayer.PlaceInitialBet();
        }

        public void DealInitialCards(IGroupOfCards cardDeck, List<IPlayer> players, List<ICard> dealerCards)
        {
            DealSingleCardTo(players, cardDeck);
            dealerCards.Add(GetTopCardFrom(cardDeck));
            DealSingleCardTo(players, cardDeck);
            dealerCards.Add(GetTopCardFrom(cardDeck));
        }

        public void PlayersPlay(IGroupOfCards cardDeck, List<IPlayer> players, ICard dealersVisibleCard)
        {
            foreach (var player in players)
            {
                player.PlayTurn(dealersVisibleCard);
                while (player.DoesNeedCard)
                {
                    player.TakeCard(GetTopCardFrom(cardDeck));
                    player.PlayTurn(dealersVisibleCard);
                }
            }
        }

        public void DealerPlays(IGroupOfCards cardDeck, List<ICard> dealerCards, IDealerStrategy dealerStrategy)
        {
            while (dealerStrategy.ShouldHit(dealerCards))
                dealerCards.Add(GetTopCardFrom(cardDeck));
        }

        public void DeterminePlayerHandOutcomes(List<ICard> dealerCards, List<IPlayer> players)
        {
            int dealerHandValue = dealerCards.GetBestCardValue();
            foreach (var player in players)
            {
                foreach (var playerHand in player.CurrentHands)
                    SetHandOutcome(playerHand, dealerHandValue);
            }
        }

        public decimal PayoutOrCollectBetsFrom(List<IPlayer> players)
        {
            decimal moneyCollected = 0;
            foreach (var player in players)
            {
                foreach (var playerHand in player.CurrentHands)
                    moneyCollected += GetOrDistributeMoneyTo(player, playerHand);
            }

            return moneyCollected;
        }

        public List<ICard> CollectCardsFrom(List<IPlayer> players)
        {
            var cardsToReturn = new List<ICard>();
            foreach (var player in players)
            {
                foreach (var playerHand in player.CurrentHands)
                    cardsToReturn.AddRange(CollectCardsFrom(playerHand.Cards));
            }

            return cardsToReturn;
        }

        public List<ICard> CollectCardsFrom(List<ICard> cards)
        {
            var cardsToReturn = new List<ICard>();
            cardsToReturn.AddRange(cards);
            cards.Clear();
            return cardsToReturn;
        }

        public void SaveCurrentHandResultsOf(List<IPlayer> players)
        {
            foreach (var player in players)
                player.SaveCurrentHands();
        }

        public void ClearHandsFrom(List<IPlayer> players)
        {
            foreach (var player in players)
                player.ClearCurrentHands();
        }

        public void DetermineToLeaveGameOrStay(List<IPlayer> players)
        {
            for (int playerIndex = players.Count - 1; playerIndex >= 0; playerIndex--)
            {
                var player = players[playerIndex];
                player.LeaveTableOrStay();
            }
        }

        private decimal GetOrDistributeMoneyTo(IPlayer player, IPlayerHand playerHand)
        {
            switch (playerHand.Outcome)
            {
                case HandOutcome.InProgress:
                    throw new InvalidOperationException("Cannot get or distribute money to hand already in play");
                case HandOutcome.Won:
                    if (playerHand.IsBlackjack)
                    {
                        player.TotalCash += Math.Round(playerHand.Bet * Constants.BlackjackBetWinMultiplier,
                            Constants.DecimalDigitsForCash);
                    }
                    else
                    {
                        player.TotalCash += Math.Round(playerHand.Bet * Constants.NonBlackjackBetWinMultiplier,
                            Constants.DecimalDigitsForCash);
                    }
                    return 0;
                case HandOutcome.Lost:
                    return playerHand.Bet;
                case HandOutcome.Pushed:
                    player.TotalCash += playerHand.Bet;
                    return 0;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void SetHandOutcome(IPlayerHand playerHand, int dealerHandValue)
        {
            int playerHandValue = playerHand.GetBestCardValue();

            if (playerHandValue > Constants.BestHandValue)
                playerHand.Outcome = HandOutcome.Lost;
            else if (dealerHandValue > Constants.BestHandValue)
                playerHand.Outcome = HandOutcome.Won;
            else if (playerHandValue == dealerHandValue)
                playerHand.Outcome = HandOutcome.Pushed;
            else if (playerHandValue > dealerHandValue)
                playerHand.Outcome = HandOutcome.Won;
            else if (playerHandValue < dealerHandValue)
                playerHand.Outcome = HandOutcome.Lost;
            else
                throw new Exception("Could not determine outcome of player's hand");
        }

        private void DealSingleCardTo(List<IPlayer> players, IGroupOfCards cardDeck)
        {
            foreach (var player in players)
                player.TakeCard(GetTopCardFrom(cardDeck));
        }

        private ICard GetTopCardFrom(IGroupOfCards cardDeck)
        {
            var cardToReturn = cardDeck.PullTopCard();
            if (cardToReturn.GetType() == typeof(NullCard))
                throw new EmptyGroupOfCardsException();

            return cardToReturn;
        }
    }
}