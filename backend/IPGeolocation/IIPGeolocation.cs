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
        /// Geolocate the Current IpAdress.
        /// </summary>
        /// <param name="httpClient">HttpClient with them the CurrentIpAdress lockup will be excecuted</param>
        /// <returns>Basic String Respone from the Result (Continet - Region - City)</returns>
        Task<string> GeolocateOwnAdress(HttpClient? httpClient = null);

        /// <summary>
        /// Geolocate an IpAdress.
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <returns>Basic String Respone from the Result (Continet - Region - City)</returns>
        Task<string> GeolocateAdress(IPAddress ipAddress);
    }
}
