using System;
using GamblingLibrary.Enums;
using GamblingLibrary.Interfaces;

namespace BlackjackSimulator
{
    public class BlackjackCardValueAssigner : ICardValueAssigner
    {
        private const int FACE_CARD_VALUE = 10;

        public int GetCardValueFor(CardType cardType, CardSuit cardSuit)
        {
            if (cardType < CardType.Jack)
                return (int) cardType + 2;

            if (cardType >= CardType.Jack && cardType < CardType.Ace)
                return FACE_CARD_VALUE;

            if (cardType == CardType.Ace)
                return Constants.AceCardValue;

            throw new ArgumentOutOfRangeException("Unrecognized card type");
        }

        public bool CanAssignNewValueFor(CardType cardType, CardSuit cardSuit, int desiredCardValue)
        {
            return cardType == CardType.Ace &&
                   (desiredCardValue == Constants.AceCardValue || desiredCardValue == Constants.AlternateAceCardValue);
        }
    }
}