using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;

namespace PuppeteerSharp.Helpers
{
    internal static class ProcessExtensions
    {
        internal static void RemoveExitedEvent(this Process process)
        {
            RemoveHandler(process, "_onExited");
            RemoveHandler(process, "onExited");
        }

        private static void RemoveHandler(Process process, string fieldId)
        {
            var eventField = typeof(Process).GetField(fieldId, BindingFlags.NonPublic | BindingFlags.Instance);

            if (eventField != null)
            {
                object obj = eventField.GetValue(process);
                var pi = process.GetType().GetProperty("Events", BindingFlags.NonPublic | BindingFlags.Instance);
                var list = (EventHandlerList)pi.GetValue(process, null);
                list.RemoveHandler(obj, list[obj]);
            }
        }
    }
}
