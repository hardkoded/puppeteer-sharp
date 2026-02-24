namespace PuppeteerSharp.Cdp.Messaging
{
    internal class AutofillTriggerRequest
    {
        public int FieldId { get; set; }

        public string FrameId { get; set; }

        public CreditCardData Card { get; set; }
    }
}
