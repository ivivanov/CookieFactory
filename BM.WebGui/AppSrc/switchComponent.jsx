export class SwitchComponent extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            isConnected: false,
        };
    }

    componentDidMount() {
    }

    render() {
        return (
            <div>
                <button disabled={this.state.isConnected}>On</button>
                <button disabled={this.state.isConnected}>Pause</button>
                <button disabled={this.state.isConnected}>Off</button>
            </div >
        );
    }
}