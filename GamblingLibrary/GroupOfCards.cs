using System;
using System.Collections.Generic;
using System.Linq;
using GamblingLibrary.Interfaces;

namespace GamblingLibrary
{
    public abstract class GroupOfCards : IGroupOfCards
    {
        public virtual List<ICard> Cards
        {
            get;
            private set;
        }

        protected GroupOfCards()
        {
            Cards = new List<ICard>();
        }

        public void Shuffle()
        {
            var randomNumberGenerator = new Random((int)DateTime.Now.Ticks);
            Cards = Cards.Select(x => new {Number = randomNumberGenerator.Next(), Item = x})
                .OrderBy(x => x.Number)
                .Select(x => x.Item)
                .ToList();
        }

        public void Cut(int numberOfCards)
        {
            var topCutOfCards = Cards.GetRange(0, numberOfCards);
            Cards.RemoveRange(0, numberOfCards);
            Cards.AddRange(topCutOfCards);
        }

        public ICard PullTopCard()
        {
            var cardToReturn = Cards.DefaultIfEmpty(new NullCard()).FirstOrDefault();
            Cards.Remove(cardToReturn);
            return cardToReturn;
        }
    }
}