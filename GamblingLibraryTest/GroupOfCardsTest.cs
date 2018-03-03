using System.Collections.Generic;
using GamblingLibrary;
using GamblingLibrary.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace GamblingLibraryTest
{
    [TestClass]
    public class GroupOfCardsTest
    {
        private readonly GroupOfCards _sut;

        public GroupOfCardsTest()
        {
            _sut = MockRepository.GeneratePartialMock<GroupOfCards>();
        }

        [TestInitialize]
        public void MyTestInitialize()
        {
            _sut.Stub(goc => goc.Cards).Return(new List<ICard> {new NullCard(), new NullCard()});
        }

        [TestMethod]
        public void When_Deck_Is_Cut_Should_Have_Same_Number_Of_Cards()
        {
            var cardCount = _sut.Cards.Count;
            _sut.Cut(1);

            Assert.AreEqual(cardCount, _sut.Cards.Count);
        }

        [TestMethod]
        public void When_Shuffled_Deck_Should_Have_Same_Number_Of_Cards()
        {
            var cardCount = _sut.Cards.Count;
            _sut.Shuffle();

            Assert.AreEqual(cardCount, _sut.Cards.Count);
        }

        [TestMethod]
        public void When_Top_Card_Is_Removed_Deck_Should_Have_One_Less_Card()
        {
            var cardCount = _sut.Cards.Count;
            _sut.PullTopCard();

            Assert.AreEqual(cardCount - 1, _sut.Cards.Count);
        }

        [TestMethod]
        public void When_Pulling_The_Top_Card_Should_Return_Null_Card_If_Deck_Is_Empty()
        {
            for (int cardIndex = 0; cardIndex < _sut.Cards.Count; cardIndex++)
                _sut.PullTopCard();

            var cardFromEmptyDeck = _sut.PullTopCard();
            Assert.IsInstanceOfType(cardFromEmptyDeck, typeof(NullCard));
        }
    }
}
