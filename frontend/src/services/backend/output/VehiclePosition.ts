/**
 * Api Output for an Vehicle Position
 */
import {VehiclePositionTripStop} from "./VehiclePositionTripStop";

export interface VehiclePosition {
    /**
     * Unique Id of this Trip.
     */
    idTrip: string;

    /**
     * Active Stops of this Trip.
     */
    stops: VehiclePositionTripStop[];
}
