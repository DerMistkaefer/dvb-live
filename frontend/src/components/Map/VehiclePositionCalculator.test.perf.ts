/**
 * @jest-environment node
 */
import {
    getAllStopPoints,
    getLiveVehiclePositionData,
    VehiclePosition
} from "../../services/backend";
import VehiclePositionCalculator from "./VehiclePositionCalculator"

jest.setTimeout(60000);
const calculator = new VehiclePositionCalculator();
let vehiclePositionData: VehiclePosition[] = [];


test('benchmark VehiclePositionCalculator', async () => {
    const stopPoints = await getAllStopPoints();
    calculator.setStopPoints(stopPoints);
    vehiclePositionData = await getLiveVehiclePositionData();
    // @ts-ignore
    await expect().benchmark({
        "v1": () => calculator.getCurrentVehiclePositions(vehiclePositionData),
    });
});
