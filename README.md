# Cookie Factory

Cookie Factory is a job interview task. It consists of multithreaded server side solution - self hosted console app powered by Kestrel. Also a client HTML GUI is added for better experience. 

### Tech
* Server: .NET Core, Kestrel, Custom Websockets middleware, Multithreading 
* GUI: ReactJS, Native js websockets, Bootstrap, Webpack, Babel

### GUI Features
* Switch
    * Start the machine
    * Pause (not implemeted)
    * Stop the machine
* Oven - on/off indicator
    * Heating - on/off indicator
    * Temperature - current temperature
* Motor - on/off indicator
    *  "Pulse..." - one full rotation indicates a pulse
* Biscuit Maker - produces one cookie for total of 2 motor rotations
    * Extruder "Pulse..."
    * Stamper "Pulse..."
* Biscuit Counter - on/off indicator
    * Bakeing - number of cookies on the production line
    * Baked - number of total baked cookies
* Diagnostics
    * Websocket connection health
    * Push notifications log

# Start the Cookie Factory
### Prerequisites
.NET Core 2.0, Node.js, Chrome browser 
### Installation
Starting ther server:
```sh
$ cd BM.Websockets.Server
$ dotnet build
$ dotnet run
```

After you see the "Ready" message dont close the terminal.

Opening the client GUI

```sh
$ cd BM.WebGui
$ npm install
$ npm start
```
Open link: http://localhost:3000/

# TODOs and Known Issues
Server:
* Pause is not implemented
* Machnie does not support restart - once started and than stopped can not be started again
* Synchronization problems when stopping the production line. Should be introduced CookieState mechanism for tracking the cookie position on the production line before marking it as baked

GUI:
* Improve Switch disable button states
* Handle browser refresh
* Auto re-connect to the server


