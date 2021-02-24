
class VehiclePositionCalculator
{
    private _cacheAdapter: any = null;

    private GetTripCurrentPosition(trip: any): GeoJSON.Position
    {
        const nowTime = Date.now();
        const stops = trip.Stops.OrderBy(x => x.StopSeqNumber).ToList();
        const stopOver = stops.LastOrDefault(stop => stop.DepartureCalculationTime < nowTime);
        const stopNext = stops.FirstOrDefault(stop => nowTime < stop.ArrivalCalculationTime);
        const stopDifference = stopNext?.StopSeqNumber - stopOver?.StopSeqNumber;
        if (stopDifference == null)
        {
            // trip did not start or is finished
            if (stopOver != null)
            {
                return this.GetStopPosition(stopOver.TriasIdStopPoint);
            }
            if (stopNext != null)
            {
                return this.GetStopPosition(stopNext.TriasIdStopPoint);
            }
            throw new Error("Possible Cache Error");
        }
        if (stopDifference !== 1)
        {
            throw new Error("Possible Calculation Error");
        }

        const minValue = stopOver.DepartureCalculationTime;
        const maxValue = stopNext.ArrivalCalculationTime;

        const idLastStop = stopOver.TriasIdStopPoint;
        const idNextStop = stopNext.TriasIdStopPoint;
        const currentPercentageBetweenStops = VehiclePositionCalculator.PercentageBetween(minValue.Value, maxValue.Value, nowTime);

        return this.GetCurrentPosition(idLastStop, idNextStop, currentPercentageBetweenStops);
    }

    private GetStopPosition(idStop: string): GeoJSON.Position
    {
        const stopPoint = this._cacheAdapter.GetStopPointById(idStop);

        return [stopPoint.Latitude, stopPoint.Longitude];
    }

    private GetCurrentPosition(idLastStop: string, idNextStop: string, currentPercentageBetweenStops: number): GeoJSON.Position
    {
        const lastStopPoint = this._cacheAdapter.GetStopPointById(idLastStop);
        const nextStopPoint = this._cacheAdapter.GetStopPointById(idNextStop);

        const currentLat = VehiclePositionCalculator.NumberWithPercentage(lastStopPoint.Latitude, nextStopPoint.Latitude, currentPercentageBetweenStops);
        const currentLon = VehiclePositionCalculator.NumberWithPercentage(lastStopPoint.Longitude, nextStopPoint.Longitude, currentPercentageBetweenStops);

        return [currentLat, currentLon];
    }

    private static NumberWithPercentage(minValue: number, maxValue: number, percentage:number): number
    {
        return ((maxValue - minValue) * percentage) + minValue;
    }

    private static PercentageBetween(minValue: Date, maxValue: Date, currentValueMilliseconds: number): number
    {
        const minValueMilliseconds = minValue.getTime();
        const maxValueMilliseconds = maxValue.getTime();
        return (currentValueMilliseconds - minValueMilliseconds) / (maxValueMilliseconds - minValueMilliseconds);
    }
}

export default VehiclePositionCalculator;
