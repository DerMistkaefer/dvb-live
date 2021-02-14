import React, {useState} from 'react';
import ReactMapboxGl, {
    Feature, GeoJSONLayer,
    Layer,
    Popup,
    RotationControl,
    ScaleControl,
    Source,
    ZoomControl
} from 'react-mapbox-gl';
import mapboxgl, {FlyToOptions} from "mapbox-gl";
import 'mapbox-gl/dist/mapbox-gl.css';
import {useQuery} from 'react-query';
import {getAllPublicTransportLines, getAllStopPoints, PublicTransportLine, StopPoint} from "../../services/backend";
import PositionControl from './PositionControl';
import './Map.css';
import {from} from "linq-to-typescript";

// Webpack production build destroys the worker class. So load separately.
if (process.env.NODE_ENV === 'production') {
    // @ts-ignore
    // eslint-disable-next-line import/no-webpack-loader-syntax
    mapboxgl.workerClass = require('worker-loader!mapbox-gl/dist/mapbox-gl-csp-worker').default;
}

mapboxgl.accessToken = 'pk.eyJ1IjoiZGVybWlzdGthZWZlciIsImEiOiJja2swYWQ0NHAwZm16Mm9rMmE3M3k2Zjk3In0.p8sQOMjTL_muHCN36uY9iA';
const Mapbox = ReactMapboxGl({
    accessToken: 'pk.eyJ1IjoiZGVybWlzdGthZWZlciIsImEiOiJja2swYWQ0NHAwZm16Mm9rMmE3M3k2Zjk3In0.p8sQOMjTL_muHCN36uY9iA'
});

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
    let linesFeatureCollection: GeoJSON.FeatureCollection<GeoJSON.MultiLineString> | null = null;
    if (lines.data != null)
    {
        linesFeatureCollection = {
            type: "FeatureCollection",
            features: from(lines.data).select(x => x.line).toArray()
        };
    }

    return (
        <Mapbox className='map-container'
                /* eslint-disable-next-line react/style-prop-object */
                style='mapbox://styles/mapbox/streets-v11'
                center={[lng, lat]}
                zoom={[zoom]}
                onDrag={onDrag}
                onStyleLoad={(map) => {
                    setMapboxMap(map);
                }}
                flyToOptions={flyToOptions}
        >
            {/* Controls */}
            <PositionControl />
            <ZoomControl/>
            <RotationControl/>
            <ScaleControl style={{marginBottom: "15px"}}/>
            {/* Lines */}
            <GeoJSONLayer data={linesFeatureCollection}
                          lineLayout={{
                              "visibility": "visible"
                          }}
                          linePaint={{
                              "line-width": 3,
                              "line-color": "#00f8ff"
                          }}
            />
            {/* StopPoints */}
            <Layer type="symbol" layout={{ "icon-image": ["get", "network"] }}>
                {stopPoints.data != null && stopPoints.data.map(stopPoint =>
                    <Feature key={stopPoint.idStopPoint}
                             onClick={markerClick.bind(this, stopPoint)}
                             onMouseEnter={onToggleHover.bind(this, 'pointer')}
                             onMouseLeave={onToggleHover.bind(this, '')}
                             coordinates={[stopPoint.longitude, stopPoint.latitude]}
                             properties={{network: "bus"}}
                             />
                )}
            </Layer>
            {selectedStopPoint && (
                <Popup key={selectedStopPoint.idStopPoint}
                       coordinates={[selectedStopPoint.longitude, selectedStopPoint.latitude]}
                       style={StyledPopup}
                >
                    {selectedStopPoint.stopPointName}
                </Popup>
            )}
        </Mapbox>
    );
}

export default Map;
