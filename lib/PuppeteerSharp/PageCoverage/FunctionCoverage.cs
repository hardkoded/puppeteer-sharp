namespace PuppeteerSharp.PageCoverage;

/// <summary>
/// Coverage data for a JavaScript function.
/// </summary>
public record FunctionCoverage
{
    /// <summary>
    /// JavaScript function name.
    /// </summary>
    public string FunctionName { get; set; }

    /// <summary>
    /// Source ranges inside the function with coverage data.
    /// </summary>
    public CoverageRange[] Ranges { get; set; }

    /// <summary>
    /// Whether coverage data for this function has block granularity.
    /// </summary>
    public bool IsBlockCoverage { get; set; }
}
