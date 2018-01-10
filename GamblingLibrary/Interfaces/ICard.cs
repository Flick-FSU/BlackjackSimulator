using GamblingLibrary.Enums;

namespace GamblingLibrary.Interfaces
{
    public interface ICard
    {
        int Value { get; }
        CardType Type { get; }
        CardSuit Suit { get; }
        void OverrideValue(int newValue);
    }
}