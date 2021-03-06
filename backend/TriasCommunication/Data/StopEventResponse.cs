using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using vdo.trias;
using DateTime = System.DateTime;

namespace DerMistkaefer.DvbLive.TriasCommunication.Data
{
    /// <summary>
    /// Response Structure for an Stop Event Request
    /// </summary>
    public class StopEventResponse
    {
        /// <summary>
        /// Id of the requested stop point.
        /// </summary>
        public string IdStopPoint { get; }

        /// <summary>
        /// Stop Events that are on the stop point.
        /// </summary>
        public IReadOnlyList<StopEventResult> StopEvents { get; }

        internal StopEventResponse(string idStopPoint, IReadOnlyList<StopEventResult> stopEvents)
        {
            IdStopPoint = idStopPoint;
            StopEvents = stopEvents;
        }

        internal StopEventResponse(StopEventResponseStructure response, string idStopPoint)
        {
            IdStopPoint = idStopPoint;
            StopEvents = response.StopEventResult.Select(x => new StopEventResult(x)).ToList();
        }
    }

    /// <summary>
    /// One stop event in a <see cref="StopEventResponse"/>
    /// </summary>
    public class StopEventResult
    {
        /// <summary>
        /// Refernece for the Operating Day of the journey.
        /// </summary>
        public DateTime OperatingDayRef { get; }

        /// <summary>
        /// Reference for the journey.
        /// </summary>
        public string JourneyRef { get; }

        /// <summary>
        /// Reference for the line.
        /// </summary>
        public string LineRef { get; }

        /// <summary>
        /// Name of the mode of the vehicle.
        /// </summary>
        public string ModeName { get; }

        /// <summary>
        /// Name of the Line.
        /// </summary>
        public string LineName { get; }

        /// <summary>
        /// Reference of the operator.
        /// </summary>
        public string OperatorRef { get; }

        /// <summary>
        /// Description of the Route.
        /// </summary>
        public string RouteDescription { get; }

        /// <summary>
        /// Reference of the Origin StopPoint from the Line/Route.
        /// </summary>
        public string OriginStopPointRef { get; }

        /// <summary>
        /// Reference of the Destination StopPoint from the Line/Route.
        /// </summary>
        public string DestinationStopPointRef { get; }

        /// <summary>
        /// Calls of the Stop Event.
        /// </summary>
        public IReadOnlyList<StopEventCall> Stops { get; }

        internal StopEventResult(DateTime operatingDayRef, string journeyRef, string lineRef, string modeName,
            string lineName, string operatorRef, string routeDescription,
            string originStopPointRef, string destinationStopPointRef,
            IReadOnlyList<StopEventCall> stops)
        {
            OperatingDayRef = operatingDayRef;
            JourneyRef = journeyRef;
            LineRef = lineRef;
            ModeName = modeName;
            LineName = lineName;
            OperatorRef = operatorRef;
            RouteDescription = routeDescription;
            OriginStopPointRef = originStopPointRef;
            DestinationStopPointRef = destinationStopPointRef;
            Stops = stops;
        }

        internal StopEventResult(StopEventResultStructure stopEventResult)
        {
            var stopEvent = stopEventResult.StopEvent;
            var stops = new List<StopEventCall>();
            if (stopEvent.PreviousCall != null)
            {
                stops.AddRange(stopEvent.PreviousCall.Select(call => new StopEventCall(call, CallType.Previous)));
            }
            stops.Add(new StopEventCall(stopEvent.ThisCall, CallType.This));
            if (stopEvent.OnwardCall != null)
            {
                stops.AddRange(stopEvent.OnwardCall.Select(call => new StopEventCall(call, CallType.Onward)));
            }
            Stops = stops;

            OperatingDayRef = stopEvent.Service.OperatingDayRef.Value != null ? Convert.ToDateTime(stopEvent.Service.OperatingDayRef.Value.Replace("T", "", StringComparison.CurrentCultureIgnoreCase), CultureInfo.InvariantCulture) : DateTime.Today;
            JourneyRef = stopEvent.Service.JourneyRef.Value;
            var serviceSection = stopEvent.Service.ServiceSection.First();
            LineRef = serviceSection.LineRef.Value;
            ModeName = serviceSection.Mode.Name?.FirstOrDefault(x => x.Language == "de")?.Text ?? "???";
            LineName = serviceSection.PublishedLineName?.FirstOrDefault(x => x.Language == "de")?.Text ?? "???";
            OperatorRef = serviceSection.OperatorRef.Value;
            RouteDescription = serviceSection.RouteDescription?.FirstOrDefault(x => x.Language == "de")?.Text ?? "???";
            OriginStopPointRef = stopEvent.Service.OriginStopPointRef.Value;
            DestinationStopPointRef = stopEvent.Service.DestinationStopPointRef.Value;
        }
    }

    /// <summary>
    /// One stop call in a <see cref="StopEventResult"/>
    /// </summary>
    public class StopEventCall
    {
        /// <summary>
        /// Reference for the stop point in the track.
        /// </summary>
        public string StopPointRef { get; }

        /// <summary>
        /// Name of the stop point.
        /// </summary>
        public string StopPointName { get; }

        /// <summary>
        /// Sequence number of the stop point in the track.
        /// </summary>
        public int StopSeqNumber { get; }

        /// <summary>
        /// Planned Bay
        /// </summary>
        public string PlannedBay { get; }

        /// <summary>
        /// Arrival TimeTable Time
        /// </summary>
        public DateTime? ArrivalTimeTableTime { get; }

        /// <summary>
        /// Arrival Estimated Time
        /// </summary>
        public DateTime? ArrivalEstimatedTime { get; }

        /// <summary>
        /// Departure TimeTable Time
        /// </summary>
        public DateTime? DepartureTimeTableTime { get; }

        /// <summary>
        /// Departure Estimated Time
        /// </summary>
        public DateTime? DepartureEstimatedTime { get; }

        /// <summary>
        /// Typ of this call in the request.
        /// </summary>
        public CallType Type { get; }

        internal StopEventCall(string stopPointRef, string stopPointName, int stopSeqNumber, string plannedBay,
            DateTime? arrivalTimeTableTime, DateTime? arrivalEstimatedTime, DateTime? departureTimeTableTime, DateTime? departureEstimatedTime,
            CallType callType)
        {
            StopPointRef = stopPointRef;
            StopPointName = stopPointName;
            StopSeqNumber = stopSeqNumber;
            PlannedBay = plannedBay;
            ArrivalTimeTableTime = arrivalTimeTableTime;
            ArrivalEstimatedTime = arrivalEstimatedTime;
            DepartureTimeTableTime = departureTimeTableTime;
            DepartureEstimatedTime = departureEstimatedTime;
            Type = callType;
        }

        internal StopEventCall(CallAtNearStopStructure call, CallType callType)
        {
            var callStop = call.CallAtStop;
            StopPointRef = callStop.StopPointRef.Value;
            StopSeqNumber = Convert.ToInt32(callStop.StopSeqNumber, CultureInfo.CurrentCulture);
            StopPointName = callStop.StopPointName?.FirstOrDefault(x => x.Language == "de")?.Text ?? "???";
            PlannedBay = callStop.PlannedBay?.FirstOrDefault(x => x.Language == "de")?.Text ?? "???";
            ArrivalTimeTableTime = DateTimeIsDefaultThenNull(callStop.ServiceArrival?.TimetabledTime);
            ArrivalEstimatedTime = DateTimeIsDefaultThenNull(callStop.ServiceArrival?.EstimatedTime);
            DepartureTimeTableTime = DateTimeIsDefaultThenNull(callStop.ServiceDeparture?.TimetabledTime);
            DepartureEstimatedTime = DateTimeIsDefaultThenNull(callStop.ServiceDeparture?.EstimatedTime);
            Type = callType;
        }

        private static DateTime? DateTimeIsDefaultThenNull(DateTime? dateTime)
        {
            return dateTime.HasValue ? dateTime.Value == default ? null : dateTime : null;
        }
    }

    /// <summary>
    /// Typ of this call in the request.
    /// </summary>
    public enum CallType
    {
        /// <summary>
        /// Previous call.
        /// </summary>
        Previous,

        /// <summary>
        /// Call of this Request.
        /// </summary>
        This,

        /// <summary>
        /// Onward call.
        /// </summary>
        Onward
    }
}
