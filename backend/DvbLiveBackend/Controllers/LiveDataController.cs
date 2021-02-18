using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using DerMistkaefer.DvbLive.Backend.ApiStructure.Output;
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
            var output = new List<VehiclePosition>();
            foreach (var trip in trips)
            {
                output.Add(new VehiclePosition
                {
                    Position = GetTripCurrentPosition(trip)
                });
            }

            return Json(output);
        }

        private Position GetTripCurrentPosition(CachedTrip trip)
        {
            var nowTime = DateTime.Now;
            var stops = trip.Stops.OrderBy(x => x.StopSeqNumber).ToList();
            var stopOver = stops.LastOrDefault(stop => stop.DepartureCalculationTime < nowTime);
            var stopNext = stops.FirstOrDefault(stop => nowTime < stop.ArrivalCalculationTime);
            var stopDifference = stopNext?.StopSeqNumber - stopOver?.StopSeqNumber;
            if (stopDifference == null)
            {
                // trip did not start or is finished
                if (stopOver != null)
                {
                    return GetStopPosition(stopOver.TriasIdStopPoint);
                }
                if (stopNext != null)
                {
                    return GetStopPosition(stopNext.TriasIdStopPoint);
                }
                throw new Exception("Possible Cache Error");
            }
            if (stopDifference != 1)
            {
                throw new Exception("Possible Calculation Error");
            }

            var minValue = stopOver.DepartureCalculationTime;
            var maxValue = stopNext.ArrivalCalculationTime;

            var idLastStop = stopOver.TriasIdStopPoint;
            var idNextStop = stopNext.TriasIdStopPoint;
            var currentPercentageBetweenStops = PercentageBetween(stopOver.DepartureCalculationTime.Value,
                stopNext.ArrivalCalculationTime.Value, nowTime);

            return GetCurrentPosition(idLastStop, idNextStop, currentPercentageBetweenStops);
        }

        private Position GetStopPosition(string idStop)
        {
            var stopPoint = _cacheAdapter.GetStopPointById(idStop);

            return new Position(decimal.ToDouble(stopPoint.Latitude), decimal.ToDouble(stopPoint.Longitude));
        }

        private Position GetCurrentPosition(string idLastStop, string idNextStop, double currentPercentageBetweenStops)
        {
            var lastStopPoint = _cacheAdapter.GetStopPointById(idLastStop);
            var nextStopPoint = _cacheAdapter.GetStopPointById(idNextStop);
            
            var currentLat = NumberWithPercentage(lastStopPoint.Latitude, nextStopPoint.Latitude, currentPercentageBetweenStops);
            var currentLon = NumberWithPercentage(lastStopPoint.Longitude, nextStopPoint.Longitude, currentPercentageBetweenStops);

            return new Position(decimal.ToDouble(currentLat), decimal.ToDouble(currentLon));
        }
        
        private decimal NumberWithPercentage(decimal minValue, decimal maxValue, double percentage)
            => NumberWithPercentage(minValue, maxValue, (decimal) percentage);
        
        private decimal NumberWithPercentage(decimal minValue, decimal maxValue, decimal percentage)
            => ((maxValue - minValue) * percentage) + minValue;
        
        private double NumberWithPercentage(double minValue, double maxValue, double percentage)
            => ((maxValue - minValue) * percentage) + minValue;

        private double PercentageBetween(DateTime minValue, DateTime maxValue, DateTime currentValue)
            => (currentValue - minValue).TotalSeconds / (maxValue - minValue).TotalSeconds;
        
        private double PercentageBetween(int minValue, int maxValue, int currentValue)
            => (currentValue - minValue) / (double)(maxValue - minValue);
    }
}
