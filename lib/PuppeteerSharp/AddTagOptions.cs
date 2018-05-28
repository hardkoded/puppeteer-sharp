﻿namespace PuppeteerSharp
{
    /// <summary>
    /// Options used by <see cref="Page.AddScriptTagAsync(AddTagOptions)"/> &amp; <see cref="Page.AddStyleTagAsync(AddTagOptions)"/>
    /// </summary>
    public class AddTagOptions
    {
        /// <summary>
        /// Url of a script to be added
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Path to the JavaScript file to be injected into frame. If its a relative path, then it is resolved relative to <see cref="System.IO.Directory.GetCurrentDirectory"/>
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Raw JavaScript content to be injected into frame
        /// </summary>
        public string Content { get; set; }
    }
}