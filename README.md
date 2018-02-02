# LibBZFlag
A C# implementation of BZFlag features in component format.

## Applications

### BZFlag.Game.Server
A bzfs implementation (BZFSPro) that makes heavy use of threading.

It's main goals are:
1. Maximize resources for active players
2. Maintainable code/messages
3. Maintain a logical game state
4. Fix as many security issues as possible
5. Disconnect networking from state
6. Fully integrated API

BZFSPro is not designed to be 100% compatible with the legacy BZFS server, 
but is designed to interact with legacy BZFlag clients.

#### Usage
BZFSPro only takes a single command line argument, the config file to load. 
The config file can be in XML, JSON, or YAML format.

On windows the server is run as a normal executable
BZFSPro.exe /path/to/the/config.yaml

On other OSs, the exe must be prefixed with the mono runtime command.
mono BZFSPro.exe /the/config/path.yaml

Please note that mono is NOT WINE, there is no emulation. C# is a bytecode
language and needs a runtime. 
The same exe runs on all supported platforms.

## Libraries

### BZFlag.ServiceLink
Access to the global list server and authentication systems.

### BZFlag.Data
Common shared data structures

### BZFlag.Math
Math functions used by both server and client

### BZFlag.Map
The world map database
### TODO:
* Properly transform group instances
* Handle Draw Info (why is it in the UV coords?!?!?!)

### BZFlag.IO.BZW
Library for reading/writing and storing bzworld objects. Reads/writes BZW files. Unpacks binary maps from servers.
#### TODO:
* BZW Read/Write Groups, instances, arcs, spheres, cones, and tetras
* Read Draw Info
* Finish Binary World Packer

### BZFlag.Game.Client
Library that implements the client portion of a bzflag game session

### BZFlag.Networking.Common
Network connection class for the bzflag protocol

#### Layout
1. TCP connections enter into an acceptance thread.
2. Once connection is accepted and validated as BZFS, it is handed over to a client connection manager thread (one of many)
3. Client connection mangers read data and handle message unpacking, pushing completed messages into the player object.
4. Main Game thread checks for updated player messages and does game processing.
5. Outbound messages are pushed into the player object to be processed by the client connection manager for that player.
5.1 Message is packed in a thread separate from the main thread, and then sent out via TCP or UDP depending on what the server wanted.
6. A single UDP acceptance thread, does simple processing on UDP connections and routes the unpacked messages to the client connection processor for reading.


## Tests

### ConnectionTester
Command line app that connects to a bzfs server (the one with the most players) and logs all received messages

### TestClient
WinForms GUI client that authenticates, connects to a bzfs server and logs a number of messages. Also allows user to send chat as observer.

### ReaderTest
Command line app that reads a bzw file into the libraries internal representation and then exports it back to bzw format for comparison.
File may not be identical but should read into bzfs/editors the same as the original.


Copyright 2018 BZFlag and Associates
