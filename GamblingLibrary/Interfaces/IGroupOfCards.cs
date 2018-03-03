using System.Collections.Generic;

namespace GamblingLibrary.Interfaces
{
    public interface IGroupOfCards
    {
        List<ICard> Cards { get; }

        void Cut(int numberOfCards);
        ICard PullTopCard();
        void Shuffle();
    }
}