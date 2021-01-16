import React, {useEffect, useState, createRef} from 'react';
import mapboxgl from 'mapbox-gl';
import 'mapbox-gl/dist/mapbox-gl.css';
import './Map.css';

mapboxgl.accessToken = 'pk.eyJ1IjoiZGVybWlzdGthZWZlciIsImEiOiJja2p6a3N1anIwOGsxMm9saXh2cHVqOWFsIn0.4Meqc5o2hXG7queAZoP3mg';

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