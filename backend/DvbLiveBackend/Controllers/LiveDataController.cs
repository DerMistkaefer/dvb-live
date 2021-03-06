using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using DerMistkaefer.DvbLive.Backend.ApiStructure.Output;
using DerMistkaefer.DvbLive.Backend.ApiStructure.OutputBuilder;
using DerMistkaefer.DvbLive.Backend.Cache.Api;
using DerMistkaefer.DvbLive.Backend.Cache.Data;
using GeoJSON.Net.Geometry;
using Microsoft.AspNetCore.Http;
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
        private readonly ICacheAdapter _cacheAdapter;

        /// <summary>
        /// Initialisation with dependencies
        /// </summary>
        public LiveDataController(ICacheAdapter cacheAdapter)
        {
            _cacheAdapter = cacheAdapter;
        }

        /// <summary>
        /// Get all vehicle positions.
        /// </summary>
        /// <response code="200">Return all known vehicle positions</response>
        [HttpGet("vehicle")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<VehiclePosition>))]
        public IActionResult GetCurrentVehiclePositions()
        {
            var trips = _cacheAdapter.GetAllActiveTrips();
            var output = trips.ConvertToApiOutput();

            return Json(output);
        }
    }
}
