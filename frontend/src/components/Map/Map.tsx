import React, {useMemo, useState} from 'react';
import ReactMapboxGl, {
    Layer,
    Popup,
    ScaleControl, Source,
} from 'react-map-gl';
import mapboxgl, {FlyToOptions} from "mapbox-gl";
import 'mapbox-gl/dist/mapbox-gl.css';
import {useQuery} from 'react-query';
import {getAllPublicTransportLines, getAllStopPoints, PublicTransportLine, StopPoint} from "../../services/backend";
import PositionControl from './PositionControl';
import './Map.css';
import {from} from "linq-to-typescript";
import VehiclePositionDisplay from "./VehiclePositionDisplay";
import {Feature} from "geojson";

// Webpack production build destroys the worker class. So load separately.
if (process.env.NODE_ENV === 'production') {
    // @ts-ignore
    // eslint-disable-next-line import/no-webpack-loader-syntax
    mapboxgl.workerClass = require('worker-loader!mapbox-gl/dist/mapbox-gl-csp-worker').default;
}

mapboxgl.accessToken = 'pk.eyJ1IjoiZGVybWlzdGthZWZlciIsImEiOiJja2swYWQ0NHAwZm16Mm9rMmE3M3k2Zjk3In0.p8sQOMjTL_muHCN36uY9iA';
const Mapbox = ReactMapboxGl;

const StyledPopup: React.CSSProperties = {
    color: "#3f618c",
    fontWeight: 400,
};

const flyToOptions: FlyToOptions = {
    speed: 0.8
};

const Map = () => {
    const [lng, setLng] = useState(13.738);
    const [lat, setLat] = useState(51.0497);
    const [zoom, setZoom] = useState(12);
    const [selectedStopPoint, setSelectedStopPoint] = useState<StopPoint|undefined>(undefined)
    const [mapboxMap, setMapboxMap] = useState<mapboxgl.Map|undefined>(undefined)

    const onDrag = () => {
        if (selectedStopPoint)
        {
            setSelectedStopPoint(undefined);
        }
    }

    const onToggleHover = (cursor: string) => {
        if (mapboxMap) {
            mapboxMap.getCanvas().style.cursor = cursor;
        }
    }

    const markerClick = (stopPoint: StopPoint) => {
        setSelectedStopPoint(stopPoint);
        setLng(stopPoint.longitude);
        setLat(stopPoint.latitude);
        setZoom(14);
    }

    const stopPoints = useQuery<StopPoint[], Error>('stopPoints', getAllStopPoints);
    const lines = useQuery<PublicTransportLine[], Error>('publicTransportLines', getAllPublicTransportLines);
    let linesFeatureCollection: GeoJSON.FeatureCollection<GeoJSON.Geometry> | undefined = useMemo(() => {
        if (lines.data != null)
        {
            return {
                type: "FeatureCollection",
                features: from(lines.data).select(x => x.line).toArray()
            };
        }
        return undefined;
    }, [lines]);
    let stopPointsCollection: GeoJSON.FeatureCollection<GeoJSON.Geometry> | undefined = useMemo(() => {
        if (stopPoints.data != null)
        {
            return {
                type: "FeatureCollection",
                features: from(stopPoints.data).select((x: StopPoint) =>
                    ({
                        id: x.idStopPoint,
                        type: "Feature",
                        properties: {
                            network: "bus",
                        },
                        geometry: {
                            type: "Point",
                            coordinates: [x.longitude, x.latitude]
                        }
                    } as Feature<GeoJSON.Point>))
                    .toArray()
            };
        }
        return undefined;

    }, [stopPoints]);

    return (
        <Mapbox id='map-container'
                mapStyle='mapbox://styles/mapbox/streets-v11'
                initialViewState={{
                    longitude: lng,
                    latitude: lat,
                    zoom: zoom
                }}
                zoom={zoom}
                onDrag={onDrag}
                onLoad={(event) => {
                    setMapboxMap(event.target);
                }}
                mapboxAccessToken={mapboxgl.accessToken}
                // flyToOptions={flyToOptions}
        >
            {/* Controls */}
            <PositionControl />
            {/*<ZoomControl/>
            <RotationControl/>*/}
            <ScaleControl style={{marginBottom: "15px"}}/>
            {/* Lines */}
            <Source type="geojson" data={linesFeatureCollection}>
                <Layer type="line"
                       layout={{
                           visibility: "visible"
                       }}
                       paint={{
                           "line-width": 3,
                           "line-color": "#00f8ff"
                       }}
               />
            </Source>
            {/* StopPoints */}
            <Source type="geojson" data={stopPointsCollection}>
                <Layer type="symbol"
                       layout={{
                           "icon-image": ["get", "network"],
                       }}
                />
            </Source>
            {/*<Layer type="symbol" layout={{ "icon-image": ["get", "network"] }}>
                {stopPoints.data != null && stopPoints.data.map(stopPoint =>
                    <Feature key={stopPoint.idStopPoint}
                             onClick={markerClick.bind(this, stopPoint)}
                             onMouseEnter={onToggleHover.bind(this, 'pointer')}
                             onMouseLeave={onToggleHover.bind(this, '')}
                             coordinates={[stopPoint.longitude, stopPoint.latitude]}
                             properties={{network: "bus"}}
                             />
                )}
            </Layer>*/}
            {selectedStopPoint && (
                <Popup key={selectedStopPoint.idStopPoint}
                       longitude={selectedStopPoint.longitude}
                       latitude={selectedStopPoint.latitude}
                       style={StyledPopup}
                >
                    {selectedStopPoint.stopPointName}
                </Popup>
            )}
            {/* Live Vehicles */}
            <VehiclePositionDisplay/>
        </Mapbox>
    );
}

export default Map;
