import * as React from 'react';
import {ControlPosition} from "react-map-gl/src/types/external";
import {useMap} from "react-map-gl";
import {useEffect, useState} from "react";

const triggerEvents = ['move'];

const positions: { [key: string]: React.CSSProperties} = {
    'top-right': {top: 10, right: 10, bottom: 'auto', left: 'auto'},
    'top-left': {top: 10, left: 10, bottom: 'auto', right: 'auto'},
    'bottom-right': {bottom: 10, right: 10, top: 'auto', left: 'auto'},
    'bottom-left': {bottom: 10, left: 10, top: 'auto', right: 'auto'}
};

const containerStyle: React.CSSProperties = {
    position: 'absolute',
    zIndex: 10,
    boxShadow: '0px 1px 4px rgba(0, 0, 0, .3)',
    border: '1px solid rgba(0, 0, 0, 0.1)',
    right: 50,
    backgroundColor: '#fff',
    opacity: 0.85,
    display: 'flex',
    flexDirection: 'row',
    alignItems: 'baseline',
    padding: '3px 7px'
};

const POSITIONS = Object.keys(positions);

export interface Props {
    position?: ControlPosition;
    style?: React.CSSProperties;
    className?: string;
    tabIndex?: number;
}

const PositionControl = (props: Props) => {
    const [longitude, setLongitude] = useState(0);
    const [latitude, setLatitude] = useState(0);
    const [zoom, setZoom] = useState(0);
    const {current: mapRef} = useMap();
    if (mapRef === undefined) {
        throw new Error("Map is not defined");
    }
    const map = mapRef?.getMap();

    const setPosition = () => {
        setLongitude(+map.getCenter().lng.toFixed(4));
        setLatitude(+map.getCenter().lat.toFixed(4));
        setZoom(+map.getZoom().toFixed(2));
    };

    useEffect(() => {
        setPosition();
        triggerEvents.forEach(event => {
            map.on(event, setPosition);
        });
        return function cleanup() {
            if (map) {
                triggerEvents.forEach(event => {
                    map.off(event, setPosition);
                });
            }
        }
    });
    const position = props.position ?? POSITIONS[1];

    return (
        <div
            tabIndex={props.tabIndex}
            style={{...containerStyle, ...positions[position!], ...props.style}}
            className={props.className}
        >
            Longitude: {longitude} | Latitude: {latitude} | Zoom: {zoom}
        </div>
    );
}

export default PositionControl;
