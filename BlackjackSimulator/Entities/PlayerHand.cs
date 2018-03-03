using System;
using System.Collections.Generic;
using System.Linq;
using BlackjackSimulator.Enums;
using BlackjackSimulator.Extensions;
using BlackjackSimulator.Interfaces;
using GamblingLibrary;
using GamblingLibrary.Interfaces;

namespace BlackjackSimulator.Entities
{
    public class PlayerHand : IPlayerHand
    {
        public List<ICard> Cards { get; }
        public decimal Bet { get; set; }
        public HandOutcome Outcome { get; set; }
        public bool IsBlackjack => Cards.IsBlackjack() && !IsASplit;
        public decimal TotalPlayerCashAfterOutcome { get; set; }
        public bool IsADoubleDown { get; set; }
        public bool IsASplit { get; private set; }

        public PlayerHand()
        {
            Cards = new List<ICard>();
        }

        public PlayerHand(bool isASplit) : this()
        {
            IsASplit = isASplit;
        }

        public List<int> GetCardValues()
        {
            return Cards.GetCardValues();
        }

        public int GetBestCardValue()
        {
            return Cards.GetBestCardValue();
        }

        public IPlayerHand Split()
        {
            if (!CanSplit())
                throw new InvalidOperationException("Cannot split the cards in the current hand");

            var newHand = new PlayerHand(true) {Bet = Bet, Outcome = HandOutcome.InProgress};
            newHand.Cards.Add(Cards.Last());
            Cards.Remove(newHand.Cards.First());
            IsASplit = true;

            return newHand;
        }

        public bool CanSplit()
        {
            return Cards.Count == 2 && Cards.First().Type == Cards.Last().Type;
        }

        public IPlayerHand GetDeepCopy()
        {
            var copyToReturn = new PlayerHand {Bet = Bet, Outcome = Outcome};
            foreach (var card in Cards)
                copyToReturn.Cards.Add(new Card(card.Type, card.Suit, new BlackjackCardValueAssigner()));

            return copyToReturn;
        }
    }
}