using GamblingLibrary.Enums;

namespace GamblingLibrary.Interfaces
{
    public interface ICardValueAssigner
    {
        int GetCardValueFor(CardType cardType, CardSuit cardSuit);
        bool CanAssignValueFor(CardType cardType, CardSuit cardSuit, int desiredCardValue);
    }
}