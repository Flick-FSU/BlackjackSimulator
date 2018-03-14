using System;
using BlackjackSimulator.Strategies.Interfaces;

namespace BlackjackSimulator.Models
{
    public class PlayerProperties
    {
        public Type PlayerStrategy; //todo: enforce constraint that Type needs to be and IPlayerStrategy
        public decimal StartingCash;
    }
}