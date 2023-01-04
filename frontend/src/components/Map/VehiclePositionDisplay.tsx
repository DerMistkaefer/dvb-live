import * as React from 'react';
import mapboxgl, {LngLatLike} from 'mapbox-gl';
import VehiclePositionCalculator from "./VehiclePositionCalculator";
import {from} from "linq-to-typescript";
import {useQuery} from "react-query";
import {
    getAllStopPoints,
    getLiveVehiclePositionData,
    StopPoint, VehiclePosition
} from "../../services/backend";
import {UseQueryResult} from "react-query/types/react/types";
import {useMap} from "react-map-gl";
import {useEffect, useState} from "react";

export interface Props {
    style?: React.CSSProperties;
    className?: string;
    tabIndex?: number;
}

function Query(props: any) {
    return props.children(useQuery(props.keyName, props.fn, props.options));
}

const markerCache: { [idTrip: string]: mapboxgl.Marker } = {};
const calculator: VehiclePositionCalculator = new VehiclePositionCalculator();
let liveVehiclePositionData: VehiclePosition[] = [];
const framesPerSecond = 24; // 24 is use in most films. https://en.wikipedia.org/wiki/Frame_rate

const VehiclePositionDisplay = (props: Props) => {
    const [enable, setEnable] = useState(false);
    const {current: mapRef} = useMap();
    if (mapRef === undefined) {
        throw new Error("Map is not defined");
    }
    const map = mapRef.getMap();

    useEffect(() => {
        setEnable(true);
        requestAnimationFrame((timestamp) => updateVehiclePosition(timestamp));
        return function cleanup() {
            setEnable(false);
        }
    }, [mapRef]);

    const updateVehiclePosition = (timestamp: number) => {
        const positions = calculator.getCurrentVehiclePositions(liveVehiclePositionData);
        const deleteCache = from(Object.keys(markerCache)).where(x => !from(positions).any(y => y.idTrip === x));

        deleteCache.toArray().forEach(x => delete markerCache[x]);

        positions.forEach((position) => {
            let marker = markerCache[position.idTrip];
            if (marker === undefined) {
                console.log(`New Marker ${position.idTrip}`);
                marker = new mapboxgl.Marker();
                markerCache[position.idTrip] = marker;
            }
            marker.setLngLat(position.position as LngLatLike);
            marker.addTo(map);
        });

        if (enable) {
            setTimeout(() => {
                requestAnimationFrame((timestamp) => updateVehiclePosition(timestamp));
            }, 1000 / framesPerSecond);
        }
    }

    return(
        <div>
            <Query keyName="stopPoints" fn={getAllStopPoints}>
                {({data}: UseQueryResult<StopPoint[]>) => {
                    data !== undefined && calculator.setStopPoints(data);
                    return "";
                } }
            </Query>
            <Query keyName="liveVehiclePositions" fn={getLiveVehiclePositionData}>
                {({data}: UseQueryResult<VehiclePosition[]>) => {
                    liveVehiclePositionData = (data !== undefined ? from(data).toArray() : []);
                    return "";
                } }
            </Query>
        </div>
    );
}

export default VehiclePositionDisplay;
