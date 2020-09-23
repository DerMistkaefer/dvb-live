# TRIAS - Travellor Realtime Information and Advisory Standard 
German: (EKAP - Echtzeit Kommunikations- und Auskunftsplattform)


## VDV Dokumentations
https://www.vdv.de/ip-kom-oev.aspx

System Architecture - https://www.vdv.de/vdv-431-1-ekap-systemarchitektur.pdfx

Trias 1.2 - https://www.vdv.de/431-2-sdsv1.2.pdfx

## OpenData Germany Saxony
https://register.opendata.sachsen.de/catalog/471/datasets/13

## VVO Trias Infos

```
# Wichtige Hinweise zur Nutzung der API zur Fahrplanauskunft
Dieser OpenService basiert auf der vom VDV (VDV-Schrift 431-2) veröffentlichten Standardschnittstelle TRIAS. 

Der  VVO bietet aus dem Portfolio dieses Standards  folgende Dienste an:

- Ortsinformation 		    - LocationInformationRequest 
- Verbindungsauskunft 		- TripRequest 
- Abfahrtstafeln 			- StopEventRequest

Die TRIAS-Dienste benötigen die RequestorRef „OpenService“. Wenn Sie eine Anwendung für unseren Service geschaffen haben und diese produktiv setzen wollen, empfehlen wir Ihnen, eine eigene RequestorRef beim VVO (opdendata@vvo-online.de) freischalten zu lassen. 

###  Bitte beachten Sie bei der Nutzung dieses OpenService:

1. Die Daten stehen für das Gebiet des VVO zur Verfügung und sind „ohne Gewähr“.
2. Es besteht kein Anspruch auf eine dauerhafte Verfügbarkeit des OpenService.
3. Die Services sind auf normale Last ausgelegt. Bei missbräuchlicher Nutzung durch automatisierte Massenanfragen o. ä. behalten wir uns die Sperrung der Dienste vor. 
```

Trias Implementation Dokumentation: https://www.dresden.de/media/pdf/wirtschaft/VVO_Beschreibung_der_Schnittstelle_API_fuer_die_Verbindungsauskunft.pdf

## Examples

Trias Url: http://efa.vvo-online.de:8080/std3/trias - Trias 1.2 (as of 18.07.2020)

Method: Post

Content-Type: text/xml

#### Stop Event Request
```xml
<?xml version="1.0" encoding="UTF-8"?>
<Trias version="1.2" xmlns="http://www.vdv.de/trias" xmlns:siri="http://www.siri.org.uk/siri" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:schemaLocation="http://www.vdv.de/trias ../../trias-xsd-v1.2/Trias.xsd">
  <ServiceRequest>
    <siri:RequestTimestamp>2018-04-11T11:00:00</siri:RequestTimestamp>
    <siri:RequestorRef>OpenService</siri:RequestorRef>
    <RequestPayload>
      <StopEventRequest>
        <Location>
          <LocationRef>
            <StopPointRef>de:14612:28</StopPointRef>
          </LocationRef>
          <DepArrTime>2018-04-22T10:30:00Z</DepArrTime>
        </Location>
        <Params>
          <NumberOfResults>15</NumberOfResults>
          <StopEventType>departure</StopEventType>
          <IncludePreviousCalls>false</IncludePreviousCalls>
          <IncludeOnwardCalls>false</IncludeOnwardCalls>
          <IncludeRealtimeData>true</IncludeRealtimeData>
        </Params>
      </StopEventRequest>
    </RequestPayload>
  </ServiceRequest>
</Trias>
```

#### Stop Fares Request 
```xml
<?xml version="1.0" encoding="UTF-8"?>
<Trias version="1.2" xmlns="http://www.vdv.de/trias" xmlns:siri="http://www.siri.org.uk/siri" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:schemaLocation="http://www.vdv.de/trias ../../trias-xsd-v1.2/Trias.xsd">
  <ServiceRequest>
    <siri:RequestTimestamp>2018-03-11T11:00:00</siri:RequestTimestamp>
    <siri:RequestorRef>OpenService</siri:RequestorRef>
    <RequestPayload>
      <FaresRequest>
        <StopFaresRequest>
          <StopPointRef>de:14612:28</StopPointRef>
        </StopFaresRequest>
      </FaresRequest>
    </RequestPayload>
  </ServiceRequest>
</Trias>
```

#### Trip Request 
```xml
<?xml version="1.0" encoding="UTF-8"?>
<Trias version="1.2" xmlns="http://www.vdv.de/trias" xmlns:siri="http://www.siri.org.uk/siri" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
  <ServiceRequest>
    <siri:RequestTimestamp>2019-10-11T11:00:00</siri:RequestTimestamp>
    <siri:RequestorRef>OpenService</siri:RequestorRef>
    <RequestPayload>
      <TripRequest>
        <Origin>
          <LocationRef>
            <StopPointRef>de:14612:28</StopPointRef>
          </LocationRef>
          <DepArrTime>2019-10-09T10:30:00Z</DepArrTime>
        </Origin>
        <Destination>
          <LocationRef>
            <StopPointRef>de:14612:204</StopPointRef>
          </LocationRef>
        </Destination>
        <Params>
          <IncludeTrackSections>true</IncludeTrackSections>
          <IncludeLegProjection>true</IncludeLegProjection>
          <IncludeIntermediateStops>true</IncludeIntermediateStops>
        </Params>
      </TripRequest>
    </RequestPayload>
  </ServiceRequest>
</Trias>
```

## XSD to C# Transformation
I am unsure if everything works but this is how requests can be executed.

In the xsd Folder:
```bash
"C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.8 Tools\xsd.exe" siri-1.4/datex2/DATEXIISchema_1_0_1_0.xsd siri-1.4/acsb/acsb_all-v0.3.xsd siri-1.4/ifopt/ifopt_allStopPlace-v0.3.xsd siri-1.4/siri.xsd Trias.xsd /c /n:vdo.trias /f /o:../ /l:CS /order
````
After creating the siri_Trias.cs file, some changes need to be made

- typeof(object) -> typeof(string)
- Untyped elements must be typed. Only Object to System.Xml.XmlElement 
- UserNeedStructure[][] -> UserNeedStructure[]
- OsmTagStructure[][] -> OsmTagStructure[]
- PointProjectionStructure[][] -> PointProjectionStructure[]
- StopInformationStructure[][] -> StopInformationStructure[]

