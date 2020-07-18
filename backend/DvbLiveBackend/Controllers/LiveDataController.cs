using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace DerMistkaefer.DvbLive.Backend.Controllers
{
    /// <summary>
    /// Controller with all routes for the live position of vehicles / lines
    /// </summary>
    [ApiController]
    [Route("live")]
    public class LiveDataController : ControllerBase
    {
        /// <summary>
        /// initialisation with dependencies
        /// </summary>
        public LiveDataController()
        {

        }
    }
}
