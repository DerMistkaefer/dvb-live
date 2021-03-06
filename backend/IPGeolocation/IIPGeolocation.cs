using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace DerMistkaefer.DvbLive.IPGeolocation
{
    /// <summary>
    /// Interface for all IpGeolocation functions.
    /// </summary>
    public interface IIpGeolocation
    {
        /// <summary>
        /// Geolocate the Current IpAddress.
        /// </summary>
        /// <param name="httpClient">HttpClient with them the CurrentIpAddress lockup will be executed</param>
        /// <returns>Basic String Response from the Result (Continent - Region - City)</returns>
        Task<string> GeolocateOwnAddress(HttpClient? httpClient = null);

        /// <summary>
        /// Geolocate an IpAddress.
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <returns>Basic String Response from the Result (Continent - Region - City)</returns>
        Task<string> GeolocateAddress(IPAddress ipAddress);
    }
}
