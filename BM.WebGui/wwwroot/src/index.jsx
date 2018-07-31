'use strict';

const e = React.createElement;

class BMEventLogger extends React.Component {
    constructor(props) {
        super(props);
        this.state = { liked: false };
    }

    render() {
        if (this.state.liked) {
            return 'You liked this.';
        }

        return (
            <button onClick={() => this.setState({ liked: true })}>
                Like



                asd
            </button>
        );
    }
}

const domContainer = document.querySelector('#index_container');
ReactDOM.render(e(BMEventLogger), domContainer);