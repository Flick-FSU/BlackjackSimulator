using System;
using GamblingLibrary;
using GamblingLibrary.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace GamblingLibraryTest
{
    [TestClass]
    public class ShoeOfStandardDecksOfCardsTest
    {
        private const int EXPECTED_MULTIPLE_SIZE = 52;
        private ICardValueAssigner _cardValueAssigner;
        private ShoeOfStandardDecksOfCards _sut;

        [TestInitialize]
        public void MyTestInitialize()
        {
            _cardValueAssigner = MockRepository.GenerateStub<ICardValueAssigner>();
        }

        [TestMethod]
        public void When_Shoe_Is_Created_Card_Count_Should_Be_A_Multiple_Of_Fifty_Two()
        {
            var numberOfDecks = new Random().Next(1, 5);
            _sut = new ShoeOfStandardDecksOfCards(_cardValueAssigner, numberOfDecks);

            Assert.AreEqual(0, _sut.Cards.Count % EXPECTED_MULTIPLE_SIZE);
        }
    }
}
