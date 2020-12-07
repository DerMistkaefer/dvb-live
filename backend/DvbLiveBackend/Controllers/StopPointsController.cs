using DerMistkaefer.DvbLive.Backend.ApiStructure.Output;
using DerMistkaefer.DvbLive.Backend.ApiStructure.OutputBuilder;
using DerMistkaefer.DvbLive.Backend.Cache.Api;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net.Mime;

namespace DerMistkaefer.DvbLive.Backend.Controllers
{
    /// <summary>
    /// Controller with all routes for stop points (bus stops / tram stops /...)
    /// </summary>
    [ApiController]
    [Route("stops")]
    public class StopPointsController : Controller
    {
        private readonly ICacheAdapter _cacheAdapter;

        /// <summary>
        /// Initialisation with dependencies
        /// </summary>
        public StopPointsController(ICacheAdapter cacheAdapter)
        {
            _cacheAdapter = cacheAdapter;
        }

        /// <summary>
        /// Get all known stop points.
        /// </summary>
        /// <response code="200">Return all known stop points</response>
        [HttpGet("all")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<StopPoint>))]
        public IActionResult GetAllStopPoints()
        {
            return Json(_cacheAdapter.GetAllStopPoints().ConvertToApiOutput());
        }
    }
}
