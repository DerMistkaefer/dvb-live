import axios from "axios";
import {StopPoint} from "./output/StopPoint";

const baseUrl = "https://dvb-live-backend.dermistkaefer.de";

export async function getAllStopPoints(): Promise<StopPoint[]>
{
    const response = await axios.get<StopPoint[]>(`${baseUrl}/stops/all`);

    return response.data;
}

export type { StopPoint };
