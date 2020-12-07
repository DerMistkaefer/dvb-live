using Microsoft.AspNetCore.Mvc;

namespace DerMistkaefer.DvbLive.Backend.Controllers
{
    /// <summary>
    /// Controller with all routes for the live position of vehicles / lines
    /// </summary>
    [ApiController]
    [Route("live")]
    public class LiveDataController : Controller
    {
        /// <summary>
        /// Initialisation with dependencies
        /// </summary>
        public LiveDataController()
        {

        }
    }
}
