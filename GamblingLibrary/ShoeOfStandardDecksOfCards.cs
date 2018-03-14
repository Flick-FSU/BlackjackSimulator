using GamblingLibrary.Interfaces;

namespace GamblingLibrary
{
    public sealed class ShoeOfStandardDecksOfCards : GroupOfCards
    {
        public ShoeOfStandardDecksOfCards(ICardValueAssigner cardValueAssigner, int numberOfDecks)
        {
            for (int deckIndex = 0; deckIndex < numberOfDecks; deckIndex++)
                Cards.AddRange(new StandardDeckOfCards(cardValueAssigner).Cards);
        }
    }
}