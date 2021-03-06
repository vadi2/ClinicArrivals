# User Guide

The Clinical Arrivals Program is mainly intended to be ignored while running - it will look
after itself. Users interact with the program for 4 different reasons:

* checking on the status of the program
* manaaging the room arrangements (ongoing configuration)
* performing initial configuration (only while setting up)
* Testing the behavior of the application

The Clinical Arrivals application is tricky to conceptualise and manage because 
it's a glue application between 3 different systems:
* A Practice Management System (PMS) - this manages the clinic and maintains the appointments 
* Twilio - an SMS sending/receiving system
* Jitsi - an open source xmpp based viedo-conferencing system

It watches and receives input from these systems and route it to the other
systems. 

## Tabs

The application has the following tabs: 

* **Status**: Summary of the overall health status of the program - check for any problems at a glance 
* **Appointments**: Detailed display of expected appointments for the day, and currently arrived patients (to help with troubleshooting)
* **Unknown Incoming Messages**: List of SMS messages that have arrived that weren't understood (see note below)
* **Sms Simulator**: Allows testing the application by simulating sending SMS messages to it (and seeing what has been sent)
* **PMS Simulator**: Allows testing the application by creating appointments
* **Room Mappings**: List of instructions for finding the room for a particular Doctor
* **Message Templates**: Controls the actual text of the messages that get sent out to patients 
* **Settings**: Application configuration 
* **About**: Information about the program 

## Checking on the status of the program

Because the application is glue between 3 others systems,issues with any of these other systems, or connectivity with them, 
can interupt the normal running of the program.

You can check on the program by watching the counts of the appointments in different statuses, and how many SMS messages
have been sent and received, and also by how long it is since any change in status in those numbers. 

In addition, for ease of review, the application lists which appointments are still expected for the day, and 
which have patients who have arrived (either in the carpark, or on video). Note that this list only includes 
patients that have a mobile phone associated with the appointment; other appointments are ignored.

Patients are asked to respond to SMS messages that are sent to them with one word answers. 
Some patients will respond out of time, or with messages that are not understood by the program.
These messages are listed for easy review, though the program always responds to these kinds of 
messages with a note that it wasn't understood and to call reception. 

See [Troubleshooting](Troubleshooting.md) for additional information.

## Managing the room configurations

If the patient is physically attending the practice, they wait out in the car park until 
they arrive, and then they are summoned into the practice by a message that the doctor is 
ready to see them. The message might read something like this:

  Dr Adam Ant is ready to see you now. Please come to room 5
  
This saves or reduces the need for the patient interacting with (and maybe waiting at) 
reception to find out where they should go, if they don't already know. 

However the PMS systems do not track which room the Doctor is in. So this information
must come from the application itself. The Room Mappings tab contains a list of 
Dcotor Names how have appointments for the day, and a text note that explains where 
to go for that doctors office. If no note is configured for the doctor, the message
will say:

  Dr Adam Ant is ready to see you now

## Performing Initial Configuration 

This screen handles configuration when the prgram is first set up, and 
shouldn't require any additional management after that. 

See:

* [Setting up the SMS phone # using Twilio](Twilio.md)
* [Program Settings](Settings.md)

## Testing the Behaviour of the Application.

Because the application is glue between 3 different systems, it can be very difficult to 
understand how it works. To help explain how it works, and test it, the application 
can be run in simulator mode. 

See [Simulator Mode](Simulator.md) for further details.



