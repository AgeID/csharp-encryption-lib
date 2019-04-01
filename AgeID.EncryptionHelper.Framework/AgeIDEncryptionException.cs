using System;

namespace AgeID
{
    internal class AgeIDEncryptionException : Exception
    {
        public AgeIDEncryptionException ()
        {
        }

        public AgeIDEncryptionException (string message)
            : base(message)
        {
        }

        //public AgeIDEncryptionException (string message, Exception inner)
        //    : base(message, inner)
        //{
        //}
    }
}
