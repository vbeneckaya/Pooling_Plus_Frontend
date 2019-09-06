import React from 'react';
import { Input } from 'semantic-ui-react';

import './style.scss';

export default class Search extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            value: props.value,
        };
    }

    handleKeyPress = e => {
        if (e.keyCode === 13) this.props.onChange(e, { value: this.state.value });
    };

    change = (e, { value }) => {
        const { isAuto } = this.props;
        this.setState({ value });
        if (isAuto || !value) this.props.onChange(e, { value });
    };

    render() {
        const { value, isAuto } = this.props;

        return (
            <Input
                icon="search"
                className="search-input"
                onKeyDown={this.handleKeyPress}
                onChange={this.change}
                placeholder={'Искать по всем полям'}
                value={isAuto ? value : this.state.value}
            />
        );
    }
}
