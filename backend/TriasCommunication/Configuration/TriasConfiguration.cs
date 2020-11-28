using Knapcode.TorSharp;
using System;

namespace DerMistkaefer.DvbLive.TriasCommunication.Configuration
{
    /// <summary>
    /// Main configuration for the TriasCommunication.
    /// </summary>
    public class TriasConfiguration
    {
        /// <summary>
        /// Main Trias-Api-Endpoint Url that should be used.
        /// </summary>
        public Uri TriasUrl { get; set; }

        /// <summary>
        /// Tort Sharp Settings for the Trias Communication.
        /// </summary>
        internal TorSharpSettings TorSharpSettings { get; }

        /// <summary>
        /// Name of the HttpCLient from the HttpClient Factory that TriasCommunicattion will be used.
        /// </summary>
        internal static string HttpClientFactoryClientName => "TriasCommunication.HttpClient";

        /// <summary>
        /// Default Constructor to load config from Json.
        /// </summary>
        public TriasConfiguration()
        {
            TriasUrl = null!;
            TorSharpSettings = DefaultTorSharpSettings;
        }

        /// <summary>
        /// Construct the default TriasConfiguration only with the Endpoint Url.
        /// </summary>
        /// <param name="triasUrl">Main Trias-Api-Endpoint Url that should be used.</param>
        public TriasConfiguration(Uri triasUrl)
        {
            TriasUrl = triasUrl;
            TorSharpSettings = DefaultTorSharpSettings;
        }

        private static TorSharpSettings DefaultTorSharpSettings => new TorSharpSettings
        {
            PrivoxySettings = new TorSharpPrivoxySettings
            {
                MaxClientConnections = 20000
            },
            TorSettings = new TorSharpTorSettings
            {
                ControlPassword = $"{HttpClientFactoryClientName}{DateTime.Now}"
            }
        };
    }
}
