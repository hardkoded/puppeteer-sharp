using System.Threading.Tasks;

namespace PuppeteerSharp.Abstractions
{
    interface IDialog
    {
        DialogType DialogType { get; set; }
        string DefaultValue { get; set; }
        string Message { get; set; }
        Task Accept(string promptText = "");
        Task Dismiss();
    }
}