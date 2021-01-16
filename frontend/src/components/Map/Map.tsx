import React, {useEffect, useState, createRef} from 'react';
import mapboxgl from 'mapbox-gl';
import 'mapbox-gl/dist/mapbox-gl.css';
import './Map.css'
if (process.env.NODE_ENV !== 'test') {
    // @ts-ignore
    // eslint-disable-next-line import/no-webpack-loader-syntax
    mapboxgl.workerClass = require('worker-loader!mapbox-gl/dist/mapbox-gl-csp-worker').default;
}

mapboxgl.accessToken = 'pk.eyJ1IjoiZGVybWlzdGthZWZlciIsImEiOiJja2swYWQ0NHAwZm16Mm9rMmE3M3k2Zjk3In0.p8sQOMjTL_muHCN36uY9iA';

const Map = () => {
    const mapContainerRef = createRef<HTMLDivElement>();

    const [lng, setLng] = useState(13.738);
    const [lat, setLat] = useState(51.0497);
    const [zoom, setZoom] = useState(12);

    // Initialize map when component mounts
    useEffect(() => {
        const map = new mapboxgl.Map({
            container: mapContainerRef.current!,
            style: 'mapbox://styles/mapbox/streets-v11',
            center: [lng, lat],
            zoom: zoom
        });

        // Add navigation control (the +/- zoom buttons)
        map.addControl(new mapboxgl.NavigationControl(), 'top-right');

        map.on('move', () => {
            setLng(+map.getCenter().lng.toFixed(4));
            setLat(+map.getCenter().lat.toFixed(4));
            setZoom(+map.getZoom().toFixed(2));
        });

        // Clean up on unmount
        return () => map.remove();
    }, []); // eslint-disable-line react-hooks/exhaustive-deps

    return (
        <div>
            <div className='sidebarStyle'>
                <div>
                    Longitude: {lng} | Latitude: {lat} | Zoom: {zoom}
                </div>
            </div>
            <div className='map-container' ref={mapContainerRef} />
        </div>
    );
};

export default Map;