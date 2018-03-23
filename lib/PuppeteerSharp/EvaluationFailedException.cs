using System;
using System.Runtime.Serialization;

namespace PuppeteerSharp
{
    [Serializable]
    public class EvaluationFailedException : Exception
    {
        public EvaluationFailedException()
        {
        }

        public EvaluationFailedException(string message) : base(message)
        {
        }

        public EvaluationFailedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected EvaluationFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}