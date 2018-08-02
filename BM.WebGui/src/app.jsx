import React, { Component } from "react";
import "./app.css";
import LogComponent from "./logComponent";

class App extends Component {
  render() {
    return (
      <div className="app">
        <LogComponent />
      </div>
    );
  }
}

export default App;
