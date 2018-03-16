using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;

namespace PuppeteerSharp.Helpers
{
    public static class ProcessExtensions
    {
        public static void RemoveExitedEvent(this Process process)
        {
            var eventField = typeof(Process).GetField("_onExited", BindingFlags.NonPublic | BindingFlags.Instance);

            if (eventField != null)
            {
                var obj = eventField.GetValue(process);
                var pi = process.GetType().GetProperty("Events", BindingFlags.NonPublic | BindingFlags.Instance);
                var list = (EventHandlerList)pi.GetValue(process, null);
                list.RemoveHandler(obj, list[obj]);
            }
        }
    }
}
