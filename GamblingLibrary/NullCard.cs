using GamblingLibrary.Enums;
using GamblingLibrary.Interfaces;

namespace GamblingLibrary
{
    public class NullCard : ICard
    {
        public int Value { get; }
        public CardType Type { get; }
        public CardSuit Suit { get; }
        public void OverrideValue(int newValue) { }
    }
}