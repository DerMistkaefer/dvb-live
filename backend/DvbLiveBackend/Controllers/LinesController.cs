using System.Collections.Generic;
using System.Net.Mime;
using DerMistkaefer.DvbLive.Backend.ApiStructure.Output;
using DerMistkaefer.DvbLive.Backend.ApiStructure.OutputBuilder;
using DerMistkaefer.DvbLive.Backend.Cache.Api;
using DerMistkaefer.DvbLive.GetPublicTransportLines.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DerMistkaefer.DvbLive.Backend.Controllers
{
    /// <summary>
    /// Controller with all routes for lines / routes of bus / trams ....
    /// </summary>
    [ApiController]
    [Route("lines")]
    public class LinesController : Controller
    {
        private readonly ICacheAdapter _cacheAdapter;

        /// <summary>
        /// Initialisation with dependencies
        /// </summary>
        public LinesController(ICacheAdapter cacheAdapter)
        {
            _cacheAdapter = cacheAdapter;
        }

        /// <summary>
        /// Get all known lines.
        /// </summary>
        /// <response code="200">Return all known lines</response>
        [HttpGet("all")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<PublicTransportLine>))]
        public IActionResult GetAllLines()
        {
            return Json(_cacheAdapter.GetAllLines());
        }
    }
}