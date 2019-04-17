namespace PuppeteerSharp
{
    /// <summary>
    /// Options to be used in <see cref="Page.PdfAsync(string, PdfOptions)"/>, <see cref="Page.PdfStreamAsync(PdfOptions)"/> and <see cref="Page.PdfDataAsync(PdfOptions)"/>
    /// </summary>
    [System.Obsolete("Use PuppeteerSharp.Abstractions.PdfOptions class instead")]
    public class PdfOptions : Abstractions.PdfOptions
    {
    }
}