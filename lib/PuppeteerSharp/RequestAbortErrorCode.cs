﻿namespace PuppeteerSharp
{
    /// <summary>
    /// Abort error codes. used by <see cref="Request.AbortAsync(RequestAbortErrorCode)"/>
    /// </summary>
    public enum RequestAbortErrorCode
    {
        /// <summary>
        /// An operation was aborted (due to user action)
        /// </summary>
        Aborted,

        /// <summary>
        /// Permission to access a resource, other than the network, was denied
        /// </summary>
        AccessDenied,

        /// <summary>
        /// The IP address is unreachable. This usually means that there is no route to the specified host or network.
        /// </summary>
        AddressUnreachable,

        /// <summary>
        /// A connection timed out as a result of not receiving an ACK for data sent.
        /// </summary>
        ConnectionAborted,

        /// <summary>
        /// A connection was closed (corresponding to a TCP FIN).
        /// </summary>
        ConnectionClosed,

        /// <summary>
        /// A connection attempt failed.
        /// </summary>
        ConnectionFailed,

        /// <summary>
        /// A connection attempt was refused.
        /// </summary>
        ConnectionRefused,

        /// <summary>
        ///  A connection was reset (corresponding to a TCP RST).
        /// </summary>
        ConnectionReset,

        /// <summary>
        /// The Internet connection has been lost.
        /// </summary>
        InternetDisconnected,

        /// <summary>
        /// The host name could not be resolved.
        /// </summary>
        NameNotResolved,

        /// <summary>
        /// An operation timed out.
        /// </summary>
        TimedOut,

        /// <summary>
        ///  A generic failure occurred.
        /// </summary>
        Failed,
    }
}