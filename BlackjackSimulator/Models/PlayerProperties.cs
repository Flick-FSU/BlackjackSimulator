using System;

namespace BlackjackSimulator.Models
{
    public class PlayerProperties
    {
        public Type PlayerStrategy; //todo: enforce constraint that Type needs to be an IPlayerStrategy
        public decimal StartingCash;
    }
}