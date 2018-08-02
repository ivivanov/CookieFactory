import React, { Component } from "react";

class LogComponent extends Component {
  constructor(props) {
    super(props);
    this.state = {
      connection: "Closed",
      log: ""
    };
    this.connectionUrl = "ws://localhost:52020/ws";
    this.socket = new WebSocket(this.connectionUrl);
  }

  componentDidMount() {
    var self = this;
    // socket = new WebSocket(connectionUrl);
    this.socket.onopen = function(event) {
      self.setState(function(prevState, props) {
        return { connection: "Open" };
      });
    };

    this.socket.onclose = function(event) {
      self.setState(function(prevState, props) {
        return { connection: "Closed" };
      });
    };

    this.socket.onerror = function(event) {
      self.setState(function(prevState, props) {
        return { log: prevState.log + "Error\n" };
      });
    };

    this.socket.onmessage = function(event) {
      self.setState(function(prevState, props) {
        return { log: prevState.log + event.data };
      });
    };
  }

  handleLogChange(){

  }

  render() {
    return (
      <div>
        <p>Connection: {this.state.connection}</p>
        <textarea
          id='commsLog'
          className='event-logger'
          value={this.state.log}
          disabled
        />
      </div>
    );
  }
}

export default LogComponent;
