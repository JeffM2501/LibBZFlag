# LibBZFlag
A library implementaiton of bzflag features

## BZFlag.Authentication
A basic list server client with authentication

## BZFlag.Data
Common shared data structures

## BZFlag.Map
The world map database
### TODO:
* Properly transform group instances
* Handle Draw Info (why is it in the UV coords?!?!?!)

## BZFlag.IO.BZW
Library for reading/writing and storing bzworld objects. Reads/writes BZW files. Unpacks binary maps from servers.
### TODO:
* BZW Read/Write Groups, instances, arcs, spheres, cones, and tetras
* Read Draw Info
* Binary World Packer

## BZFlag.Game.Client
Library that implements the client portion of a bzflag game session

## BZFlag.Networking.Client
Network connection class for bzflag clients

## BZFlag.Networking.Server
Network connection class for BZFS Servers

### Layout

1. TCP connections enter into an acceptance thread.
2. Once connection is accepted and validated as BZFS, it is handed over to a client connection manager thread (one of many)
3. Client connection mangers read data and handle message unpacking, pushing completed messages into the player object.
4. Main Game thread checks for updated player messages and does game processing.
5. Outbound messages are pushed into the player object to be processed by the client connection manager for that player.
5.1 Message is packed in a thread seperate from the main thread, and then sent out via TCP or UDP depending on what the server wanted.
6. A single UDP acceptance thread, does simple processing on UDP connections and routes the unpacked messages to the client connection processor for reading.

## BZFlag.Networking.Common
Network message implementations.

## Tests

### ConnectionTester
Command line app that conencts to a bzfs server (the one with the most players) and logs all received messages

### TestClient
WinForms GUI client that authenticates, connects to a bzfs server and logs a number of messages. Also allows user to send chat as observer.

### ReaderTest
Command line app that reads a bzw file into the libraries internal representation and then exports it back to bzw format for comparision.
File may not be idenitcal but should read into bzfs/editors the same as the origonal.
