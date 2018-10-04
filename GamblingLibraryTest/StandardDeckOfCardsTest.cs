using System.Linq;
using GamblingLibrary;
using GamblingLibrary.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace GamblingLibraryTest
{
    [TestClass]
    public class StandardDeckOfCardsTest
    {
        private const int EXPECTED_SIZE_OF_DECK = 52;
        private StandardDeckOfCards _sut;

        [TestInitialize]
        public void MyTestInitialize()
        {
            var cardValueAssigner = new Mock<ICardValueAssigner>().Object;
            _sut = new StandardDeckOfCards(cardValueAssigner);
        }

        [TestMethod]
        public void When_Deck_Is_Created_Should_Have_Fifty_Two_Cards_In_Card_Collection()
        {
            Assert.AreEqual(EXPECTED_SIZE_OF_DECK, _sut.Cards.Count);
        }

        [TestMethod]
        public void When_Deck_Is_Cut_With_A_Nonzero_Integer_Should_Have_A_Different_Top_Card()
        {
            var topCard = _sut.Cards.First();
            _sut.Cut(2);

            Assert.AreNotEqual(topCard, _sut.PullTopCard());
        }

        [TestMethod]
        public void When_Standard_Deck_Is_Created_Should_Not_Have_Duplicate_Cards()
        {
            Assert.AreEqual(EXPECTED_SIZE_OF_DECK, _sut.Cards.Distinct().Count());
        }
    }
}
