import * as React from 'react';
import {Map} from 'mapbox-gl';
import {AnchorLimits} from 'react-mapbox-gl/lib-esm/util/types';
import {MapContext} from "react-mapbox-gl";

const triggerEvents = ['move'];

const positions = {
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
    position?: AnchorLimits;
    style?: React.CSSProperties;
    className?: string;
    tabIndex?: number;
    map: Map;
}

export interface State {
    longitude: number;
    latitude: number;
    zoom: number;
}

class PositionControl extends React.Component<Props, State> {
    public static defaultProps = {
        position: POSITIONS[1]
    };

    public state = {
        longitude: 0,
        latitude: 0,
        zoom: 0
    };

    public componentDidMount() {
        this.setPosition();

        triggerEvents.forEach(event => {
            this.props.map.on(event, this.setPosition);
        });
    }

    public componentWillUnmount() {
        if (this.props.map) {
            triggerEvents.forEach(event => {
                this.props.map.off(event, this.setPosition);
            });
        }
    }

    public render() {
        const {style, position, className, tabIndex} = this.props;
        const {longitude, latitude, zoom} = this.state;

        return (
            <div
                tabIndex={tabIndex}
                style={{...containerStyle, ...positions[position!], ...style}}
                className={className}
            >
                Longitude: {longitude} | Latitude: {latitude} | Zoom: {zoom}
            </div>
        );
    }

    private setPosition = () => {
        const {map} = this.props;

        this.setState({
            longitude: +map.getCenter().lng.toFixed(4),
            latitude: +map.getCenter().lat.toFixed(4),
            zoom: +map.getZoom().toFixed(2)
        });
    };
}

// tslint:disable-next-line:no-any
export function withMap(Component: React.ComponentClass<any>) {
    return function MappedComponent<T>(props: T) {
        return (
            <MapContext.Consumer>
                {map => <Component map={map} {...props} />}
            </MapContext.Consumer>
        );
    };
}

export default withMap(PositionControl);
