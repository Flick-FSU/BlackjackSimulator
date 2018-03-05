using System;
using BlackjackSimulator.Strategies.Interfaces;

namespace BlackjackSimulator.Models
{
    public class PlayerProperties
    {
        public IPlayerStrategy PlayerStrategy;
        public decimal StartingCash;
    }
}