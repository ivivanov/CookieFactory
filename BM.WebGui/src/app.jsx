import React, { Component } from "react";
import "./app.css";

class App extends Component {
  constructor(props) {
    super(props);

    this.state = {
      connection: "Closed",
      log: "",
      isConnected: false
    };

    this.connectionUrl = "ws://localhost:5006/ws";
    this.socket = new WebSocket(this.connectionUrl);
    this.start = this.start.bind(this);
    this.pause = this.pause.bind(this);
    this.stop = this.stop.bind(this);
  }

  start() {
    this.socket.send("start");
  }

  pause() {
    this.socket.send("pause");
  }

  stop() {
    this.socket.send("stop");
  }

  componentDidMount() {
    var self = this;
    this.socket.onopen = function(event) {
      self.setState(function(prevState, props) {
        return {
          connection: "Open",
          isConnected: true
        };
      });
    };

    this.socket.onclose = function(event) {
      self.setState(function(prevState, props) {
        return {
          connection: "Closed",
          log: prevState.log + event.data ? event.data : "disconected" + "\n",
          isConnected: false
        };
      });
    };

    this.socket.onerror = function(event) {
      self.setState(function(prevState, props) {
        return {
          log: prevState.log + "Error\n",
          isConnected: false
        };
      });
    };

    this.socket.onmessage = function(event) {
      debugger;
      self.setState(function(prevState, props) {
        return { log: prevState.log + event.data + "\n" };
      });
    };
  }

  render() {
    return (
      <div className="app">
        <div>Connection: {this.state.connection}</div>

        <div>
          <button disabled={!this.state.isConnected} onClick={this.start}>
            Start
          </button>
          <button disabled={!this.state.isConnected} onClick={this.pause}>
            Pause
          </button>
          <button disabled={!this.state.isConnected} onClick={this.stop}>
            Stop
          </button>
        </div>

        <textarea
          id="commsLog"
          className="event-logger"
          value={this.state.log}
          disabled
        />
      </div>
    );
  }
}

export default App;
