import { SwitchComponent } from './switchComponent';

let e = React.createElement,
    connectionUrl = "ws://localhost:52020/ws",
    commsLog = document.getElementById("commsLog"),
    socket;

class LogComponent extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            connection: 'Closed',
            log: 'Empty log'
        };
    }

    componentDidMount() {
        this.OpenWebSocketConnection();
    }

    OpenWebSocketConnection() {
        var self = this;
        socket = new WebSocket(connectionUrl);
        socket.onopen = function (event) {
            self.setState((prevState) => {
                return { connection: 'Open' }
            });
        };
        socket.onclose = function (event) {
            self.setState((prevState) => {
                return { connection: 'Closed' }
            });;
        };
        socket.onerror = function (event) {
            self.setState((prevState) => {
                return { connection: 'Error' }
            });
        };
        socket.onmessage = function (event) {
            debugger
            self.setState((prevState) => {
                return { log: prevState.log + event.data }
            });;
        };
    }

    render() {
        return (
            <div>
                <p>Connection: {this.state.connection}</p>
                <textarea id='commsLog' className='bm-event-logger' defaultValue={this.state.log} />
                <SwitchComponent />
            </div >
        );
    }
}

