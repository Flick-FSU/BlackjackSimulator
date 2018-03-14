using GamblingLibrary.Enums;
using GamblingLibrary.Interfaces;

namespace GamblingLibrary
{
    public sealed class StandardDeckOfCards : GroupOfCards
    {
        public StandardDeckOfCards(ICardValueAssigner cardValueAssigner)
        {
            for (int cardIndex = (int) CardType.Two; cardIndex <= (int) CardType.Ace; cardIndex++)
            {
                for (int suitIndex = (int) CardSuit.Diamonds; suitIndex <= (int) CardSuit.Spades; suitIndex++)
                    Cards.Add(new Card((CardType) cardIndex, (CardSuit) suitIndex, cardValueAssigner));
            }
        }
    }
}