namespace PuppeteerSharp;

/// <summary>
/// New document information.
/// </summary>
public class NewDocumentScriptEvaluation(string documentIdentifierIdentifier)
{
    /// <summary>
    /// New document identifier.
    /// </summary>
    public string Identifier { get; set; } = documentIdentifierIdentifier;
}
