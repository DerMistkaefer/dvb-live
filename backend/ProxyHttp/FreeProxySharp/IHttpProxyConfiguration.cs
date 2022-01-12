namespace DerMistkaefer.DvbLive.ProxyHttp.FreeProxySharp
{
	/// <summary>
	/// HttpClient configuration
	/// </summary>
	public interface IHttpProxyConfiguration
	{
        int RetryFirstDelay { get; }
        int Retry { get; }
        IHttpProxyServer[] Proxies { get; set; }
    }

    /// <summary>
    /// HttpClient proxy configuration
    /// </summary>
    public interface IHttpProxyServer
    {
        string Ip { get; }
        int Port { get; }
        string Note { get; }
    }
}
