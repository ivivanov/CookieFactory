import React, { Component } from 'react';
import './app.css';

class App extends Component {
  constructor(props) {
    super(props);

    this.state = {
      connection: 'Closed',
      log: '',
      isConnected: false,
      oven: 'Off',
      temperature: 0,
      heating: 'Off',
      motor: 'Off',
      biscuitMaker: 'Off',
      hideMotorPulse: true,
      hideExtruderPulse: true,
      hideStamperPulse: true,
      pulseHideTimeout: 500,
      biscuitCounter: 'Off',
      baking: 0,
      baked: 0
    };

    this.connectionUrl = 'ws://localhost:5006/ws';
    this.socket = new WebSocket(this.connectionUrl);
    this.start = this.start.bind(this);
    this.pause = this.pause.bind(this);
    this.stop = this.stop.bind(this);
  }

  start() {
    this.socket.send('start');
    this.setState(function () { return { isStarted: true } });
  }

  pause() {
    this.socket.send('pause');
  }

  stop() {
    this.socket.send('stop');
  }

  componentDidMount() {
    var self = this;
    this.socket.onopen = function (event) {
      self.setState(function (prevState, props) {
        return {
          connection: 'Open',
          isConnected: true
        };
      });
    };

    this.socket.onclose = function (event) {
      self.setState(function (prevState, props) {
        return {
          connection: 'Closed',
          log: prevState.log + event.data ? event.data : 'disconected' + '\n',
          isConnected: false,
          isStarted: false,
          oven: 'Off',
          temperature: 0,
          heating: 'Off',
          motor: 'Off',
          biscuitMaker: 'Off',
          hideMotorPulse: true,
          hideExtruderPulse: true,
          hideStamperPulse: true,
          biscuitCounter: 'Off',
          baking: 0,
          baked: 0
        };
      });
    };

    this.socket.onerror = function (event) {
      self.setState(function (prevState, props) {
        return {
          log: prevState.log + 'Error. Please make sure the server is started\n',
          isConnected: false
        };
      });
    };

    this.socket.onmessage = function (event) {
      self.setState(function (prevState, props) {
        return { log: prevState.log + event.data + '\n' };
      });

      self.messageHandler(event.data);
    };
  }

  messageHandler(message) {
    debugger
    var parsedMessage = message.split(':');
    var module = parsedMessage[0];
    var value = parsedMessage[1];

    switch (module) {
      case 'OvenModule':
        this.setState({ oven: value });
        break;
      case 'HeatingModule':
        this.setState({ heating: value });
        break;
      case 'ThermometerModule':
        this.setState({ temperature: value });
        break;
      case 'MotorModule':
        if (value === "On" || value === "Off") {
          this.setState({ motor: value });
        }

        //Make the text "pulse"
        if (value == "Pulse...") {
          this.setState({
            hideMotorPulse: false,
          });

          setTimeout(() => {
            this.setState({
              hideMotorPulse: true,
            });
          }, this.state.pulseHideTimeout);
        }
        break;
      case 'BiscuitMakerModule':
        if (value === "On" || value === "Off") {
          this.setState({ biscuitMaker: value });
        }

        if (value.includes("Extruder")) {
          this.setState({
            hideExtruderPulse: false,
          });

          setTimeout(() => {
            this.setState({
              hideExtruderPulse: true,
            });
          }, this.state.pulseHideTimeout);
        }

        if (value.includes("Stamper")) {
          this.setState({
            hideStamperPulse: false,
          });

          setTimeout(() => {
            this.setState({
              hideStamperPulse: true,
            });
          }, this.state.pulseHideTimeout);
        }

        break;
      case "BiscuitCounterModule":
        if (value === "On" || value === "Off") {
          this.setState({ biscuitCounter: value });
        }

        if (parsedMessage[1] == "Baking") {
          this.setState({ baking: parsedMessage[2] });
        }

        if (parsedMessage[1] == "Baked") {
          this.setState({ baked: parsedMessage[2] });
        }
        break;
      default:
        this.setState(function (prevState, props) {
          var unknownCommand = 'Unknown module : ' + module;
          return { log: prevState.log + unknownCommand + '\n' };
        });

    }
  }

  render() {
    return (
      <div className='app container'>
        <h3 className='pb-3'>The Biscuit Machine</h3>
        <div className='row p-1'>
          <div className='col'>
            <div className='btn-group' role='group'>
              <button className='btn btn-primary' disabled={!this.state.isConnected && !this.state.isStarted} onClick={this.start}>Start</button>
              <button className='btn btn-primary' disabled>Pause</button>
              <button className='btn btn-primary' disabled={!this.state.isStarted} onClick={this.stop}>Stop</button>
              <button className={!this.state.isConnected ? 'btn btn-danger' : 'btn btn-success'} disabled>Websocket connection: {this.state.connection}</button>
            </div>
          </div>
        </div>

        <div className='row p-1'>
          <div className='col-4'>

            <div className='input-group mb-3'>
              <div className='input-group-prepend'>
                <span className='input-group-text' id='basic-addon3'>Oven</span>
              </div>
              <input type='text' className='form-control' aria-describedby='basic-addon3' value={this.state.oven} disabled />
            </div>
            <div className='row'>
              <div className='col-7'>
                <div className='input-group mb-3'>
                  <div className='input-group-prepend'>
                    <span className='input-group-text' id='basic-addon3'>Heating</span>
                  </div>
                  <input type='text' className='form-control' aria-describedby='basic-addon3' value={this.state.heating} disabled />
                </div>
              </div>
              <div className='col-5'>
                <div className='input-group mb-3'>
                  <input type='text' className='form-control' aria-describedby='basic-addon3' value={this.state.temperature} disabled />
                  <div className='input-group-append'>
                    <span className='input-group-text' id='basic-addon3'>&#x2103;</span>
                  </div>
                </div>
              </div>
            </div>

          </div>
          <div className='col-4'>
            <div className='input-group mb-3'>
              <div className='input-group-prepend'>
                <span className='input-group-text' id='basic-addon3'>Motor</span>
              </div>
              <input type='text' className='form-control' aria-describedby='basic-addon3' value={this.state.motor} disabled />
            </div>
            <div className='row'>
              <div className='col-7'>
                <div className='input-group mb-3'>
                  <div className='input-group-prepend'>
                    <span className='input-group-text' id='basic-addon3'>Rotation</span>
                  </div>
                  <span type='text' className='form-control faded' aria-describedby='basic-addon3'><span className={this.state.hideMotorPulse ? 'hide' : ''}>Pulse...</span></span>
                </div>
              </div>
            </div>

          </div>
          <div className='col-4'>
            <div className='input-group mb-3'>
              <div className='input-group-prepend'>
                <span className='input-group-text' id='basic-addon3'>Biscuit Maker</span>
              </div>
              <input type='text' className='form-control' aria-describedby='basic-addon3' value={this.state.biscuitMaker} disabled />
            </div>
            <div className='row'>
              <div className='col'>
                <div className='input-group mb-3'>
                  <div className='input-group-prepend'>
                    <span className='input-group-text' id='basic-addon3'>Extruder</span>
                  </div>
                  <span type='text' className='form-control' aria-describedby='basic-addon3'><span className={this.state.hideExtruderPulse ? 'hide' : ''}>Pulse...</span></span>
                </div>
              </div>
              <div className='col'>
                <div className='input-group mb-3'>
                  <div className='input-group-prepend'>
                    <span className='input-group-text' id='basic-addon3'>Stamper</span>
                  </div>
                  <span type='text' className='form-control' aria-describedby='basic-addon3'><span className={this.state.hideStamperPulse ? 'hide' : ''}>Pulse...</span></span>
                </div>
              </div>
            </div>
          </div>

        </div>
        <div className='row p-1'>
          <div className='col-4 offset-8'>
            <div className='input-group mb-3'>
              <div className='input-group-prepend'>
                <span className='input-group-text' id='basic-addon3'>Biscuit Counter</span>
              </div>
              <input type='text' className='form-control' aria-describedby='basic-addon3' value={this.state.biscuitCounter} disabled />
            </div>
            <div className='row'>
              <div className='col'>
                <div className='input-group mb-3'>
                  <div className='input-group-prepend'>
                    <span className='input-group-text' id='basic-addon3'>Baking</span>
                  </div>
                  <input type='text' className='form-control' aria-describedby='basic-addon3' value={this.state.baking} disabled />
                </div>
              </div>
              <div className='col'>
                <div className='input-group mb-3'>
                  <div className='input-group-prepend'>
                    <span className='input-group-text' id='basic-addon3'>Baked</span>
                  </div>
                  <input type='text' className='form-control' aria-describedby='basic-addon3' value={this.state.baked} disabled />
                </div>
              </div>
            </div>
          </div>
        </div>

        <div className='row p-1'>
          <div className='col'>
            <div className="form-group">
              <label htmlFor="log">Push notifications log</label>
              <textarea id='log' className="form-control" rows="3" value={this.state.log} disabled />
            </div>
          </div>
        </div>
      </div >
    );
  }
}

export default App;
