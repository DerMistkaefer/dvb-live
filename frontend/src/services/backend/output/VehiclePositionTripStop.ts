/**
 * Api Output for an Trip Stop of the Vehicle Position Output
 */
export interface VehiclePositionTripStop {
    /**
     * Unique Id for this Stop Point
     */
    idStopPoint: string;

    /**
     * Arrival Time on this Stop of an Trip.
     */
    arrivalTime: number | null;

    /**
     * Departure Tie on this Stop of an Trip.
     */
    departureTime: number | null;
}
