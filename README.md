# ClinicArrivals 
 
This program is writtent to fit into an existing GP clinic workflow
to help a GP manage their workflow during the COVID-19 crisis in Australia.

* [Product documentation](documentation/Documentation.md)

## Contributions

Contributions are welcome, either as Pull Requests or bug 
reports. Or you can joint the chat at 
https://chat.fhir.org/#narrow/stream/227888-clinic-arrivals

If you are a user, contact RACGP IT forum for advice/support.

## Developer Documemntation

### Local Storage 

The application stores information locally in the location [XX?]

The current [settings](Settings.md) are stored there. 

In addition, the communication record of past appointments is 
also stored there. Deleting this information or moving the 
application to a different PC without moving this data will
result in resetting any ongoing messaging flows with the 
patients.

### Kernel

The core of the program is in MessageEngine.cs. This is where
the application queries for the current appointment list from the
PMS, and also scans for incoming sms messages to process

Every X seconds (as specified in the settings, the application 
searches for all appointments on the current day. Then it works 
through the appointments


### Testing


