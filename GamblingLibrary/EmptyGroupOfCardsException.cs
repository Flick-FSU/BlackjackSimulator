using System;

namespace GamblingLibrary
{
    public class EmptyGroupOfCardsException : Exception
    {
        public override string Message => "There are no more cards left in the collection";
    }
}