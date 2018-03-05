using System;
using System.Collections.Generic;
using System.Linq;
using BlackjackSimulator;
using BlackjackSimulator.Entities;
using BlackjackSimulator.Entities.Interfaces;
using BlackjackSimulator.Enums;
using BlackjackSimulator.Strategies.Interfaces;
using GamblingLibrary;
using GamblingLibrary.Enums;
using GamblingLibrary.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace BlackjackSimulatorTest
{
    [TestClass]
    public class GameManagerTest
    {
        private IGameManager _sut;
        private IGroupOfCards _mockGroupOfCards;
        private readonly BlackjackCardValueAssigner _blackjackCardValueAssigner;

        public GameManagerTest()
        {
            _blackjackCardValueAssigner = new BlackjackCardValueAssigner();
        }

        [TestInitialize]
        public void MyTestInitialize()
        {
            _sut = new GameManager();
            _mockGroupOfCards = MockRepository.GenerateMock<IGroupOfCards>();
            _mockGroupOfCards.Stub(mgoc => mgoc.PullTopCard()).Return(new Card(CardType.Eight, CardSuit.Clubs, new BlackjackCardValueAssigner()));
        }

        [TestMethod]
        public void When_Calling_Place_Your_Bets_Should_Tell_All_Players_To_Place_An_Initial_Bet()
        {
            var mockPlayer1 = GetMockBlackjackPlayer();
            var mockPlayer2 = GetMockBlackjackPlayer();
            var players = new List<IPlayer> {mockPlayer1, mockPlayer2};

            _sut.PlaceYourBets(players);

            mockPlayer1.AssertWasCalled(mp1 => mp1.PlaceInitialBet());
            mockPlayer2.AssertWasCalled(mp2 => mp2.PlaceInitialBet());
        }

        [TestMethod]
        public void When_Null_Card_Encountered_During_Initial_Deal_Should_Throw_Exception()
        {
            var players = new List<IPlayer> { GetMockBlackjackPlayer() };
            var dealerCards = new List<ICard>();
            var mockGroupOfCards = MockRepository.GenerateMock<IGroupOfCards>();
            mockGroupOfCards.Stub(mgoc => mgoc.PullTopCard()).Return(new NullCard());

            Assert.ThrowsException<EmptyGroupOfCardsException>(() =>
                _sut.DealInitialCards(mockGroupOfCards, players, dealerCards));
        }

        [TestMethod]
        public void When_Finished_Initial_Deal_Dealer_Should_Have_Two_Cards()
        {
            var players = new List<IPlayer> {GetMockBlackjackPlayer()};
            var dealerCards = new List<ICard>();

            _sut.DealInitialCards(_mockGroupOfCards, players, dealerCards);

            Assert.AreEqual(2, dealerCards.Count);
        }

        [TestMethod]
        public void When_Starting_Game_All_Players_Should_Take_Two_Cards()
        {
            var player1 = GetStrictMockBlackjackPlayer();
            var player2 = GetStrictMockBlackjackPlayer();

            player1.Expect(p1 => p1.TakeCard(null)).IgnoreArguments().Repeat.Twice();
            player2.Expect(p2 => p2.TakeCard(null)).IgnoreArguments().Repeat.Twice();
            player1.Replay();
            player2.Replay();
            var players = new List<IPlayer> { player1, player2 };
            var dealerCards = new List<ICard>();
            _sut.DealInitialCards(_mockGroupOfCards, players, dealerCards);

            player1.VerifyAllExpectations();
            player2.VerifyAllExpectations();
        }

        [TestMethod]
        public void When_Players_Playing_Should_Tell_All_Players_To_Play_At_Least_Once()
        {
            var player1 = GetMockBlackjackPlayer();
            var player2 = GetMockBlackjackPlayer();
            player1.Stub(p1 => p1.DoesNeedCard).Return(false);
            player2.Stub(p2 => p2.DoesNeedCard).Return(false);
            player1.Expect(p1 => p1.PlayTurn(null)).IgnoreArguments();
            player2.Expect(p2 => p2.PlayTurn(null)).IgnoreArguments();
            player1.Replay();
            player2.Replay();
            var players = new List<IPlayer> {player1, player2};
            _sut.PlayersPlay(_mockGroupOfCards, players, new NullCard());

            player1.VerifyAllExpectations();
            player2.VerifyAllExpectations();
        }

        [TestMethod]
        public void When_Players_Playing_With_Null_Card_Should_Throw_Exception()
        {
            var player1 = GetMockBlackjackPlayer();
            player1.Stub(p1 => p1.DoesNeedCard).Return(true);
            var players = new List<IPlayer> {player1};
            var mockGroupOfCards = MockRepository.GenerateMock<IGroupOfCards>();
            mockGroupOfCards.Stub(mgoc => mgoc.PullTopCard()).Return(new NullCard());

            Assert.ThrowsException<EmptyGroupOfCardsException>(() =>
                _sut.PlayersPlay(mockGroupOfCards, players, new NullCard()));
        }

        [TestMethod]
        public void When_Dealer_Playing_And_Strategy_Says_To_Hit_Once_Should_Increment_Card_Count_By_One()
        {
            var dealerStrategy = MockRepository.GenerateMock<IDealerStrategy>();
            var dealerCards = new List<ICard>();
            dealerStrategy.Stub(ds => ds.ShouldHit(dealerCards)).Return(true).Repeat.Once();
            int initialDealerCardCount = dealerCards.Count;

            _sut.DealerPlays(_mockGroupOfCards, dealerCards, dealerStrategy);

            Assert.AreEqual(initialDealerCardCount + 1, dealerCards.Count);
        }

        [TestMethod]
        public void When_Dealer_Playing_And_Strategy_Says_To_Hit_Twice_Should_Increment_Card_Count_By_Two()
        {
            var dealerStrategy = MockRepository.GenerateMock<IDealerStrategy>();
            var dealerCards = new List<ICard>();
            dealerStrategy.Stub(ds => ds.ShouldHit(dealerCards)).Return(true).Repeat.Twice();
            int initialDealerCardCount = dealerCards.Count;

            _sut.DealerPlays(_mockGroupOfCards, dealerCards, dealerStrategy);

            Assert.AreEqual(initialDealerCardCount + 2, dealerCards.Count);
        }

        [TestMethod]
        public void When_Dealer_Playing_And_Strategy_Says_Not_To_Hit_Card_Count_Should_Stay_The_Same()
        {
            var dealerStrategy = MockRepository.GenerateMock<IDealerStrategy>();
            var dealerCards = new List<ICard>();
            dealerStrategy.Stub(ds => ds.ShouldHit(dealerCards)).Return(false);
            int initialDealerCardCount = dealerCards.Count;

            _sut.DealerPlays(_mockGroupOfCards, dealerCards, dealerStrategy);

            Assert.AreEqual(initialDealerCardCount, dealerCards.Count);
        }

        [TestMethod]
        public void When_Winners_Determined_Should_Have_No_Players_Hands_In_InPlay_State()
        {
            var player1Hand1 = new PlayerHand { Outcome = HandOutcome.InProgress, Bet = 0 };
            var player1Hand2 = new PlayerHand { Outcome = HandOutcome.InProgress, Bet = 0 };
            var player2Hand = new PlayerHand { Outcome = HandOutcome.InProgress, Bet = 0 };
            var player1HandList = new List<IPlayerHand> {player1Hand1, player1Hand2};
            var player2HandList = new List<IPlayerHand> {player2Hand};
            var mockPlayer1 = MockRepository.GenerateMock<IPlayer>();
            mockPlayer1.Stub(mp1 => mp1.CurrentHands).Return(player1HandList);
            var mockPlayer2 = MockRepository.GenerateMock<IPlayer>();
            mockPlayer2.Stub(mp2 => mp2.CurrentHands).Return(player2HandList);
            var players = new List<IPlayer> { mockPlayer1, mockPlayer2 };

            _sut.DeterminePlayerHandOutcomes(new List<ICard>(), players);

            foreach (var player in players)
            {
                foreach (var playerHand in player.CurrentHands)
                    Assert.AreNotEqual(HandOutcome.InProgress, playerHand.Outcome);
            }
        }

        [TestMethod]
        public void When_Player_Hand_Is_Busted_Should_Set_Player_Hand_To_Lost_State()
        {
            var cards = new List<ICard>
            {
                new Card(CardType.Jack, CardSuit.Clubs, _blackjackCardValueAssigner),
                new Card(CardType.Jack, CardSuit.Clubs, _blackjackCardValueAssigner),
                new Card(CardType.Three, CardSuit.Clubs, _blackjackCardValueAssigner)
            };
            var players = GetSinglePlayerCollectionContaining(cards);

            _sut.DeterminePlayerHandOutcomes(new List<ICard>(), players);

            Assert.AreEqual(HandOutcome.Lost, players.First().CurrentHands.First().Outcome);
        }

        [TestMethod]
        public void When_Player_Hand_Is_Busted_And_Dealer_Hand_Is_Busted_Should_Set_Player_Hand_To_Lost_State()
        {
            var playerCards = new List<ICard>
            {
                new Card(CardType.Jack, CardSuit.Clubs, _blackjackCardValueAssigner),
                new Card(CardType.Jack, CardSuit.Clubs, _blackjackCardValueAssigner),
                new Card(CardType.Three, CardSuit.Clubs, _blackjackCardValueAssigner)
            };
            var players = GetSinglePlayerCollectionContaining(playerCards);

            var dealerCards = new List<ICard>
            {
                new Card(CardType.Jack, CardSuit.Clubs, _blackjackCardValueAssigner),
                new Card(CardType.Jack, CardSuit.Clubs, _blackjackCardValueAssigner),
                new Card(CardType.Three, CardSuit.Clubs, _blackjackCardValueAssigner)
            };

            _sut.DeterminePlayerHandOutcomes(dealerCards, players);

            Assert.AreEqual(HandOutcome.Lost, players.First().CurrentHands.First().Outcome);
        }

        [TestMethod]
        public void
            When_Player_Hand_Value_Is_Less_Than_Dealer_Hand_Value_And_Dealer_Hasnt_Busted_Should_Set_To_Lost_State()
        {
            var playerCards = new List<ICard>
            {
                new Card(CardType.Jack, CardSuit.Clubs, _blackjackCardValueAssigner),
                new Card(CardType.Jack, CardSuit.Clubs, _blackjackCardValueAssigner)
            };
            var players = GetSinglePlayerCollectionContaining(playerCards);

            var dealerCards = new List<ICard>
            {
                new Card(CardType.Jack, CardSuit.Clubs, _blackjackCardValueAssigner),
                new Card(CardType.Ace, CardSuit.Clubs, _blackjackCardValueAssigner)
            };

            _sut.DeterminePlayerHandOutcomes(dealerCards, players);

            Assert.AreEqual(HandOutcome.Lost, players.First().CurrentHands.First().Outcome);
        }

        [TestMethod]
        public void When_Player_Hand_Value_Is_Equal_To_Dealer_Hand_Value_Should_Set_To_Push_State()
        {
            var playerCards = new List<ICard>
            {
                new Card(CardType.Jack, CardSuit.Clubs, _blackjackCardValueAssigner),
                new Card(CardType.Jack, CardSuit.Clubs, _blackjackCardValueAssigner)
            };
            var players = GetSinglePlayerCollectionContaining(playerCards);

            var dealerCards = new List<ICard>
            {
                new Card(CardType.Jack, CardSuit.Clubs, _blackjackCardValueAssigner),
                new Card(CardType.Queen, CardSuit.Clubs, _blackjackCardValueAssigner)
            };

            _sut.DeterminePlayerHandOutcomes(dealerCards, players);

            Assert.AreEqual(HandOutcome.Pushed, players.First().CurrentHands.First().Outcome);
        }

        [TestMethod]
        public void
            When_Player_Hand_Value_Is_Greater_Than_Dealer_Hand_Value_And_Player_Hasnt_Busted_Should_Set_To_Won_State()
        {
            var playerCards = new List<ICard>
            {
                new Card(CardType.Jack, CardSuit.Clubs, _blackjackCardValueAssigner),
                new Card(CardType.Ace, CardSuit.Clubs, _blackjackCardValueAssigner)
            };
            var players = GetSinglePlayerCollectionContaining(playerCards);

            var dealerCards = new List<ICard>
            {
                new Card(CardType.Jack, CardSuit.Clubs, _blackjackCardValueAssigner),
                new Card(CardType.Queen, CardSuit.Clubs, _blackjackCardValueAssigner)
            };

            _sut.DeterminePlayerHandOutcomes(dealerCards, players);

            Assert.AreEqual(HandOutcome.Won, players.First().CurrentHands.First().Outcome);
        }

        [TestMethod]
        public void When_Paying_Or_Collecting_Bet_Of_A_Hand_With_A_Push_Outcome_Bet_Should_Return_To_Player_Total_Cash()
        {
            const decimal initialPlayerTotalCash = 100;
            const decimal handBet = 10;
            var players = GetSinglePlayerListWithSingleCompletedHand(initialPlayerTotalCash, handBet, HandOutcome.Pushed);

            _sut.PayoutOrCollectBetsFrom(players);
            Assert.AreEqual(initialPlayerTotalCash + handBet, players.First().CurrentTotalCash);
        }

        [TestMethod]
        public void When_Paying_Bet_Of_Winning_Non_Blackjack_Hand_Should_Pay_Amount_Equal_To_Bet()
        {
            const decimal initialPlayerTotalCash = 100;
            const decimal handBet = 10;
            var players = GetSinglePlayerListWithSingleCompletedHand(initialPlayerTotalCash, handBet, HandOutcome.Won);

            _sut.PayoutOrCollectBetsFrom(players);
            Assert.AreEqual(initialPlayerTotalCash + handBet * Constants.NonBlackjackBetWinMultiplier,
                players.First().CurrentTotalCash);
        }

        [TestMethod]
        public void When_Paying_Bet_Of_Winning_Blackjack_Hand_Should_Pay_Amount_Equal_To_One_And_A_Half_Times_Bet()
        {
            const decimal initialPlayerTotalCash = 100;
            const decimal handBet = 10;
            var mockPlayerHand = MockRepository.GenerateMock<IPlayerHand>();
            mockPlayerHand.Stub(mph => mph.Bet).Return(handBet);
            mockPlayerHand.Stub(mph => mph.Outcome).Return(HandOutcome.Won);
            mockPlayerHand.Stub(mph => mph.IsBlackjack).Return(true);
            var mockPlayerStrategy = MockRepository.GenerateMock<IPlayerStrategy>();
            var player = new Player(initialPlayerTotalCash, mockPlayerStrategy);
            player.CurrentHands.Add(mockPlayerHand);
            var players = new List<IPlayer> {player};

            _sut.PayoutOrCollectBetsFrom(players);
            Assert.AreEqual(initialPlayerTotalCash + handBet * Constants.BlackjackBetWinMultiplier,
                players.First().CurrentTotalCash);
        }

        [TestMethod]
        public void When_Collecting_Bet_Of_Losing_Hand_Should_Return_Bet_Amount()
        {
            const decimal initialPlayerTotalCash = 100;
            const decimal handBet = 10;
            var players = GetSinglePlayerListWithSingleCompletedHand(initialPlayerTotalCash, handBet, HandOutcome.Lost);

            Assert.AreEqual(handBet, _sut.PayoutOrCollectBetsFrom(players));
        }

        [TestMethod]
        public void When_Paying_Or_Collecting_Bet_Of_An_InPlay_Hand_Should_Throw_An_Exception()
        {
            const decimal initialPlayerTotalCash = 100;
            const decimal handBet = 10;
            var players = GetSinglePlayerListWithSingleCompletedHand(initialPlayerTotalCash, handBet, HandOutcome.InProgress);

            Assert.ThrowsException<InvalidOperationException>(() => _sut.PayoutOrCollectBetsFrom(players));
        }

        [TestMethod]
        public void When_Collecting_Cards_From_Players_Should_Return_Card_Count_Equal_To_All_Cards_In_Players_Hands()
        {
            var player1Hand1 = new PlayerHand();
            player1Hand1.Cards.AddRange(new List<ICard> {new NullCard(), new NullCard()});
            var player1Hand2 = new PlayerHand();
            player1Hand2.Cards.AddRange(new List<ICard> {new NullCard(), new NullCard()});
            var player2Hand = new PlayerHand();
            player2Hand.Cards.AddRange(new List<ICard> { new NullCard(), new NullCard() });
            var player1 = new Player(500, MockRepository.GenerateMock<IPlayerStrategy>());
            player1.CurrentHands.Add(player1Hand1);
            player1.CurrentHands.Add(player1Hand2);
            var player2 = new Player(500, MockRepository.GenerateMock<IPlayerStrategy>());
            player2.CurrentHands.Add(player2Hand);
            var players = new List<IPlayer> {player1, player2};

            Assert.AreEqual(6, _sut.CollectCardsFrom(players).Count);
        }

        [TestMethod]
        public void When_Cards_Collected_From_Card_List_Should_Return_Collection_Containing_Same_Card_Count()
        {
            var card1 = new NullCard();
            var card2 = new NullCard();
            var cards = new List<ICard> { card1, card2 };
            int initialCardCount = cards.Count;

            Assert.AreEqual(initialCardCount, _sut.CollectCardsFrom(cards).Count);
        }

        [TestMethod]
        public void When_Cards_Collected_From_Card_List_Should_Return_Collection_Containing_Same_Cards()
        {
            var card1 = new NullCard();
            var card2 = new NullCard();
            var cards = new List<ICard>{card1, card2};

            var returnedCards = _sut.CollectCardsFrom(cards);
            foreach (var card in cards)
                Assert.IsTrue(returnedCards.Contains(card));
        }

        [TestMethod]
        public void When_Cards_Collected_From_Dealer_Should_Clear_Dealer_Cards()
        {
            var card1 = new NullCard();
            var card2 = new NullCard();
            var cards = new List<ICard> { card1, card2 };

            _sut.CollectCardsFrom(cards);
            Assert.IsFalse(cards.Any());
        }

        [TestMethod]
        public void When_Saving_Current_Hands_Results_For_Player_Should_Tell_All_Players_To_Save_CurrentHands()
        {
            var mockPlayer1 = MockRepository.GenerateMock<IPlayer>();
            var mockPlayer2 = MockRepository.GenerateMock<IPlayer>();
            var mockPlayers = new List<IPlayer> {mockPlayer1, mockPlayer2};

            _sut.SaveCurrentHandResultsOf(mockPlayers);
            mockPlayer1.AssertWasCalled(mp1 => mp1.SaveCurrentHands());
            mockPlayer2.AssertWasCalled(mp2 => mp2.SaveCurrentHands());
        }

        [TestMethod]
        public void When_Clearing_Players_Hands_Should_Tell_All_Players_To_Clear_Hands()
        {
            var mockPlayer1 = MockRepository.GenerateMock<IPlayer>();
            var mockPlayer2 = MockRepository.GenerateMock<IPlayer>();
            var mockPlayers = new List<IPlayer> { mockPlayer1, mockPlayer2 };

            _sut.ClearHandsFrom(mockPlayers);
            mockPlayer1.AssertWasCalled(mp1 => mp1.ClearCurrentHands());
            mockPlayer2.AssertWasCalled(mp2 => mp2.ClearCurrentHands());
        }

        [TestMethod]
        public void When_Deciding_To_Remain_In_Game_Should_Tell_Players_To_Decide_Whether_To_Leave_Or_Stay()
        {
            var mockPlayer1 = MockRepository.GenerateMock<IPlayer>();
            var mockPlayer2 = MockRepository.GenerateMock<IPlayer>();
            var mockPlayers = new List<IPlayer> { mockPlayer1, mockPlayer2 };

            _sut.DetermineToLeaveGameOrStay(mockPlayers);
            mockPlayer1.AssertWasCalled(mp1 => mp1.LeaveTableOrStay());
            mockPlayer2.AssertWasCalled(mp2 => mp2.LeaveTableOrStay());
        }

        private static List<IPlayer> GetSinglePlayerListWithSingleCompletedHand(decimal initialPlayerTotalCash, decimal bet, HandOutcome handOutcome)
        {
            var playerHand = new PlayerHand {Bet = bet, Outcome = handOutcome};
            var mockPlayerStrategy = MockRepository.GenerateMock<IPlayerStrategy>();
            var player = new Player(initialPlayerTotalCash, mockPlayerStrategy);
            player.CurrentHands.Add(playerHand);
            return new List<IPlayer> { player };
        }

        private IPlayer GetMockBlackjackPlayer()
        {
            var hands = new List<IPlayerHand>();
            var blackjackPlayer = MockRepository.GenerateMock<IPlayer>();
            blackjackPlayer.Stub(bp => bp.CurrentHands).Return(hands);

            return blackjackPlayer;
        }

        private IPlayer GetStrictMockBlackjackPlayer()
        {
            var hands = new List<IPlayerHand>();
            var blackjackPlayer = MockRepository.GenerateStrictMock<IPlayer>();
            blackjackPlayer.Stub(bp => bp.CurrentHands).Return(hands);

            return blackjackPlayer;
        }

        private static List<IPlayer> GetSinglePlayerCollectionContaining(List<ICard> cards)
        {
            var playerHand = new PlayerHand {Outcome = HandOutcome.InProgress, Bet = 0};
            playerHand.Cards.AddRange(cards);
            var playerHandList = new List<IPlayerHand> {playerHand};
            var mockPlayer = MockRepository.GenerateMock<IPlayer>();
            mockPlayer.Stub(mp => mp.CurrentHands).Return(playerHandList);
            var players = new List<IPlayer> {mockPlayer};
            return players;
        }
    }
}
