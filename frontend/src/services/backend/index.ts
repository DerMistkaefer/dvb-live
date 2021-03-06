import axios from "axios";
import {PublicTransportLine} from "./output/PublicTransportLine";
import {StopPoint} from "./output/StopPoint";
import { VehiclePosition } from "./output/VehiclePosition";

let baseUrl = "https://dvb-live-backend.dermistkaefer.de";

if (!process.env.NODE_ENV || process.env.NODE_ENV === 'development')
{
    try {
        const devUrl = "http://localhost:11744";
        axios.get(`${devUrl}/hc`).then(response => {
                if (response.status === 200) {
                    baseUrl = devUrl;
                }
        });
    }
    catch (Error) {}
}

export async function getAllPublicTransportLines(): Promise<PublicTransportLine[]>
{
    const response = await axios.get<PublicTransportLine[]>(`${baseUrl}/lines/all`);

    return response.data;
}

export async function getAllStopPoints(): Promise<StopPoint[]>
{
    const response = await axios.get<StopPoint[]>(`${baseUrl}/stops/all`);

    return response.data;
}

export async function getLiveVehiclePositionData(): Promise<VehiclePosition[]>
{
    const response = await axios.get<VehiclePosition[]>(`${baseUrl}/live/vehicle`);

    return response.data;
}

export type { PublicTransportLine, StopPoint, VehiclePosition };
