import * as React from 'react';
import mapboxgl, {LngLatLike, Map} from 'mapbox-gl';
import {MapContext} from "react-mapbox-gl";
import VehiclePositionCalculator from "./VehiclePositionCalculator";
import {from} from "linq-to-typescript";
import {useQuery} from "react-query";
import {
    getAllStopPoints,
    getLiveVehiclePositionData,
    StopPoint, VehiclePosition
} from "../../services/backend";
import {UseQueryResult} from "react-query/types/react/types";

export interface Props {
    style?: React.CSSProperties;
    className?: string;
    tabIndex?: number;
    map: Map;
}

export interface State {

}

function Query(props: any) {
    return props.children(useQuery(props.keyName, props.fn, props.options));
}

class VehiclePositionDisplay extends React.Component<Props, State> {

    private enable: boolean = false;
    private markerCache: { [idTrip: string]: mapboxgl.Marker } = {};
    private calculator: VehiclePositionCalculator;
    private liveVehiclePositionData: VehiclePosition[] = [];

    constructor(props: Props) {
        super(props);
        this.calculator = new VehiclePositionCalculator();
    }

    public componentDidMount() {
        this.enable = true;
        requestAnimationFrame((timestamp) => this.updateVehiclePosition(timestamp));
    }

    public componentWillUnmount() {
        this.enable = false;
    }

    public render() {
        return(
            <div>
                <Query keyName="stopPoints" fn={getAllStopPoints}>
                    {({data}: UseQueryResult<StopPoint[]>) => {
                        data !== undefined && this.calculator.setStopPoints(data);
                        return "";
                    } }
                </Query>
                <Query keyName="liveVehiclePositions" fn={getLiveVehiclePositionData}>
                    {({data}: UseQueryResult<VehiclePosition[]>) => {
                        this.liveVehiclePositionData = (data !== undefined ? from(data).toArray() : []);
                        return "";
                    } }
                </Query>
            </div>
        );
    }

    private updateVehiclePosition(timestamp: number) {
        const map = this.props.map;
        const positions = this.calculator.getCurrentVehiclePositions(this.liveVehiclePositionData);
        const deleteCache = from(Object.keys(this.markerCache)).where(x => !from(positions).any(y => y.idTrip === x));

        deleteCache.toArray().forEach(x => delete this.markerCache[x]);

        positions.forEach((position) => {
           let marker = this.markerCache[position.idTrip];
           if (marker === undefined) {
               console.log(`New Marker ${position.idTrip}`);
               marker = new mapboxgl.Marker();
               this.markerCache[position.idTrip] = marker;
           }
           marker.setLngLat(position.position as LngLatLike);
           marker.addTo(map);
        });

        if (this.enable) {
            requestAnimationFrame((timestamp) => this.updateVehiclePosition(timestamp));
        }
    }
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

export default withMap(VehiclePositionDisplay);
