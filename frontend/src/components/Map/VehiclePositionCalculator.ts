import {from} from "linq-to-typescript";
import {StopPoint, VehiclePosition} from "../../services/backend";

export interface VehiclePositionCalculated {
    /**
     * Unique Id of this Trip.
     */
    idTrip: string;

    /**
     * Current Position of this Trip.
     */
    position: GeoJSON.Position;
}

class VehiclePositionCalculator
{
    private stopPoints: { [idStopPoint: string]: GeoJSON.Position; } = {};

    public setStopPoints(stopPoints: StopPoint[]): void
    {
        stopPoints.forEach((stopPoint) => {
            this.stopPoints[stopPoint.idStopPoint] = [stopPoint.longitude, stopPoint.latitude];
        });
    }

    public getCurrentVehiclePositions(liveData: VehiclePosition[]): VehiclePositionCalculated[]
    {
        //console.log("run");
        const nowMillis = Date.now();
        const nowSeconds = Math.floor(nowMillis / 1000);
        return from(liveData).select((trip) => {
            //const t1 = performance.now();
            const position = this.getTripCurrentPosition(trip, nowMillis, nowSeconds);
            //const t2 = performance.now();
            //console.log(`Call getTripCurrentPosition ${trip.idTrip} took ${t2 - t1} millis.`)
            return {
                idTrip: trip.idTrip,
                position: position
            }
        }).toArray()
    }

    private getTripCurrentPosition(trip: VehiclePosition, nowMillis: number, nowSeconds: number): GeoJSON.Position
    {
        const stops = from(trip.stops)
        const stopOver = stops.lastOrDefault(stop => stop.departureTime != null && stop.departureTime <= nowSeconds);
        const stopNext = stops.firstOrDefault(stop => stop.arrivalTime != null && nowSeconds <= stop.arrivalTime);
        if (stopNext == null || stopOver == null)
        {
            // trip did not start or is finished
            if (stopOver != null)
            {
                return this.GetStopPosition(stopOver.idStopPoint);
            }
            else if (stopNext != null)
            {
                return this.GetStopPosition(stopNext.idStopPoint);
            }
            throw new Error("Failed Cache");
        }

        const minValue = stopOver.departureTime! * 1000;
        const maxValue = stopNext.arrivalTime! * 1000;

        const idLastStop = stopOver.idStopPoint;
        const idNextStop = stopNext.idStopPoint;
        const currentPercentageBetweenStops = VehiclePositionCalculator.PercentageBetween(minValue, maxValue, nowMillis);

        return this.GetCurrentPosition(idLastStop, idNextStop, currentPercentageBetweenStops);
    }

    private GetCurrentPosition(idLastStop: string, idNextStop: string, currentPercentageBetweenStops: number): GeoJSON.Position
    {
        const lastStopPoint = this.GetStopPosition(idLastStop);
        const nextStopPoint = this.GetStopPosition(idNextStop);

        const currentLon = VehiclePositionCalculator.NumberWithPercentage(lastStopPoint[0], nextStopPoint[0], currentPercentageBetweenStops);
        const currentLat = VehiclePositionCalculator.NumberWithPercentage(lastStopPoint[1], nextStopPoint[1], currentPercentageBetweenStops);

        return [currentLon, currentLat];
    }

    private GetStopPosition(idStop: string): GeoJSON.Position
    {
        return this.stopPoints[idStop];
    }

    private static NumberWithPercentage(minValue: number, maxValue: number, percentage:number): number
    {
        return ((maxValue - minValue) * percentage) + minValue;
    }

    private static PercentageBetween(minValue: number, maxValue: number, currentValue: number): number
    {
        const number = (currentValue - minValue) / (maxValue - minValue);
        return isFinite(number) ? number : 0;
    }
}

export default VehiclePositionCalculator;
