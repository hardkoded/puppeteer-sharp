using System.Threading.Tasks;

namespace CefSharp.DevTools.Dom
{
    /// <summary>
    /// The EventTarget interface is implemented by objects that can receive events and may have listeners for them.
    /// In other words, any target of events implements the three methods associated with this interface.
    /// </summary>
    public class EventTarget : DomHandle
    {
        internal EventTarget(string className, JSHandle jsHandle) : base(className, jsHandle)
        {
        }

        // TODO: Add support for removeEventListener
        /// <summary>
        /// AddEventListenerAsync
        /// </summary>
        /// <param name="eventType">A case-sensitive string representing the event type to listen for.</param>
        /// <param name="functionName">name of the function that was created using <see cref="DevToolsContext.ExposeFunctionAsync(string, System.Action)"/></param>
        /// <returns>Task</returns>
        public Task AddEventListenerAsync(string eventType, string functionName) => EvaluateFunctionInternalAsync(
                @"(element, eventType, functionName) =>
                {
                    element.addEventListener(eventType, (evt) => { let f = window[functionName]; f?.(evt); }, false);
                }",
                eventType,
                functionName);
    }
}
