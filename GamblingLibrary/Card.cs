using GamblingLibrary.Enums;
using GamblingLibrary.Interfaces;

namespace GamblingLibrary
{
    public class Card : ICard
    {
        public int Value { get; private set; }

        public CardType Type { get; }
        public CardSuit Suit { get; }

        private readonly ICardValueAssigner _cardValueAssigner;

        public Card(CardType type, CardSuit suit, ICardValueAssigner cardValueAssigner)
        {
            Suit = suit;
            Type = type;
            _cardValueAssigner = cardValueAssigner;
            Value = _cardValueAssigner.GetCardValueFor(Type, Suit);
        }

        public void OverrideValue(int newValue)
        {
            if (_cardValueAssigner.CanAssignValueFor(Type, Suit, newValue))
                Value = newValue;
        }
    }
}