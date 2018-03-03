using System.Collections.Generic;
using System.Linq;
using GamblingLibrary.Enums;
using GamblingLibrary.Interfaces;

namespace BlackjackSimulator.Extensions
{
    public static class CardCollectionExtensions
    {
        public static List<int> GetCardValues(this List<ICard> cards)
        {
            var cardValuesToReturn = new List<int>();
            if (cards.All(c => c.Type != CardType.Ace))
            {
                int valueOfCards = 0;
                cards.ForEach(c => valueOfCards += c.Value);
                cardValuesToReturn.Add(valueOfCards);
            }
            else
            {
                SetAllAceCardsToAlternateValue(cards);

                for (int aceCardIndex = 0; aceCardIndex < cards.Count(c => c.Type == CardType.Ace); aceCardIndex++)
                {
                    cardValuesToReturn.Add(GetCardsValueWithAceModified(cards, aceCardIndex, Constants.AlternateAceCardValue));
                    cardValuesToReturn.Add(GetCardsValueWithAceModified(cards, aceCardIndex, Constants.AceCardValue));
                }
            }

            return cardValuesToReturn.Distinct().ToList();
        }

        public static int GetBestCardValue(this List<ICard> cards)
        {
            var cardValues = cards.GetCardValues();
            var targetCardValue = cardValues.Where(cv =>
                cv >= Constants.DealersMinimumTargetHandValue && cv <= Constants.BestHandValue);

            return targetCardValue.Any() ? targetCardValue.First() : cardValues.OrderBy(cv => cv).First();
        }

        public static bool IsBlackjack(this List<ICard> cards)
        {
            return cards.Count == 2 && cards.GetBestCardValue() == Constants.BestHandValue;
        }

        private static void SetAllAceCardsToAlternateValue(List<ICard> cards)
        {
            for (int aceCardIndex = 0; aceCardIndex < cards.Count(c => c.Type == CardType.Ace); aceCardIndex++)
            {
                ICard aceCardToModify = cards.Where(c => c.Type == CardType.Ace).ElementAt(aceCardIndex);
                aceCardToModify.OverrideValue(Constants.AlternateAceCardValue);
            }
        }

        private static int GetCardsValueWithAceModified(List<ICard> cards, int aceCardIndex, int aceCardValue)
        {
            int valueOfCards = 0;
            ICard aceCardToModify = cards.Where(c => c.Type == CardType.Ace).ElementAt(aceCardIndex);
            aceCardToModify.OverrideValue(aceCardValue);
            cards.ForEach(c => valueOfCards += c.Value);
            return valueOfCards;
        }
    }
}