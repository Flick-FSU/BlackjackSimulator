using System;
using System.Collections.Generic;
using System.Linq;
using BlackjackSimulator.Entities.Interfaces;
using BlackjackSimulator.Enums;
using BlackjackSimulator.Strategies.Interfaces;
using GamblingLibrary.Enums;
using GamblingLibrary.Interfaces;

namespace BlackjackSimulator.Entities
{
    public class Player : IPlayer
    {
        public List<IPlayerHand> CurrentHands { get; }
        public List<IPlayerHand> HandHistory { get; }
        public decimal CurrentTotalCash { get; set; }
        public decimal StartingCash { get; set; }
        public bool DoesNeedCard { get; private set; }
        public IPlayerHand InPlayHand => CurrentHands[_currentInPlayHandIndex];
        public bool IsAtTable => _currentDealer != null;
        public string StrategyName => _playerStrategy.GetType().Name;

        private int _currentInPlayHandIndex;
        private IDealer _currentDealer;
        private readonly IPlayerStrategy _playerStrategy;

        public Player(decimal startingCash, IPlayerStrategy playerStrategy)
        {
            StartingCash = CurrentTotalCash = startingCash;
            _playerStrategy = playerStrategy;
            CurrentHands = new List<IPlayerHand>();
            HandHistory = new List<IPlayerHand>();
            _currentInPlayHandIndex = 0;
            DoesNeedCard = false;
        }

        public void JoinTableWith(IDealer dealer)
        {
            _currentDealer = dealer;
            _currentDealer.Register(this);
        }

        public void TakeCard(ICard card)
        {
            InPlayHand.Cards.Add(card);
            DoesNeedCard = false;
        }

        public void PlaceInitialBet()
        {
            CurrentHands.Add(new PlayerHand
            {
                Bet = _playerStrategy.GetInitialBetAmount(HandHistory.LastOrDefault(), CurrentTotalCash, _currentDealer.TableSettings),
                Outcome = HandOutcome.InProgress
            });

            CurrentTotalCash -= InPlayHand.Bet;
        }

        public void PlayTurn(ICard visibleCard)
        {
            if (_playerStrategy.ShouldSplit(InPlayHand, visibleCard) && CanSplitInPlayHand())
            {
                DoesNeedCard = true;
                SplitCurrentHand();
            }
            else if (_playerStrategy.ShouldDoubleDown(InPlayHand, visibleCard) &&
                     CanDoubleDownInPlayHand())
            {
                DoesNeedCard = true;
                DoubleDownOnCurrentHand();
            }
            else if (_playerStrategy.ShouldHit(InPlayHand, visibleCard) && CanHitOnInPlayHand())
                DoesNeedCard = true;
            else if (_currentInPlayHandIndex < CurrentHands.Count - 1)
            {
                ++_currentInPlayHandIndex;
                PlayTurn(visibleCard);
            }
        }

        public void SaveCurrentHands()
        {
            foreach (var hand in CurrentHands)
            {
                var handForHistory = hand.GetDeepCopy();
                handForHistory.TotalPlayerCashAfterOutcome = CurrentTotalCash;
                HandHistory.Add(handForHistory);
            }
        }

        public void LeaveTableOrStay()
        {
            if (!_playerStrategy.ShouldLeaveTable(CurrentTotalCash, _currentDealer.TableSettings))
                return;

            _currentDealer.Unregister(this);
            _currentDealer = null;
        }

        public void ClearCurrentHands()
        {
            CurrentHands.Clear();
            _currentInPlayHandIndex = 0;
        }

        private bool CanSplitInPlayHand()
        {
            return InPlayHand.CanSplit() && HasEnoughCashToDoubleCurrentHandBet();
        }

        private bool CanDoubleDownInPlayHand()
        {
            return HasEnoughCashToDoubleCurrentHandBet() && !HasBusted() && !InPlayHand.IsASplit &&
                   !InPlayHand.IsADoubleDown;
        }

        private bool CanHitOnInPlayHand()
        {
            return !HasBusted() && (!InPlayHand.IsASplit || InPlayHand.Cards.First().Type != CardType.Ace) &&
                   !InPlayHand.IsADoubleDown;
        }

        private bool HasBusted()
        {
            return InPlayHand.GetCardValues().Count ==
                   InPlayHand.GetCardValues().Count(cv => cv > Constants.BestHandValue);
        }

        private void DoubleDownOnCurrentHand()
        {
            CurrentTotalCash -= InPlayHand.Bet;
            InPlayHand.Bet *= 2;
            InPlayHand.IsADoubleDown = true;
        }

        private void SplitCurrentHand()
        {
            CurrentHands.Add(InPlayHand.Split());
            CurrentTotalCash -= InPlayHand.Bet;
        }

        private bool HasEnoughCashToDoubleCurrentHandBet()
        {
            return CurrentTotalCash >= InPlayHand.Bet;
        }
    }
}