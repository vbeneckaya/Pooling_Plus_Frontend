import React from 'react';
import { Input } from 'semantic-ui-react';
import { withTranslation } from 'react-i18next';

import './style.scss';

class Search extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            value: '',
        };
    }

    componentWillMount() {
        //this.timer = null;
    }

    triggerChange = () => {
        const {value} = this.state;

        this.props.onChange(null, {value});
    };

    handleChange = (e, {value}) => {
        //clearTimeout(this.timer);

        this.setState({value});

        //this.timer = setTimeout(this.triggerChange, 300);
    };

    handleKeyPress = e => {
        if (e.keyCode === 13) {
            clearTimeout(this.timer);
            this.triggerChange();
        }
    };

    /*change = (e, { value }) => {
        const { isAuto } = this.props;
        this.setState({ value });
        if (isAuto || !value) this.props.onChange(e, { value });
    };*/

    render() {
        const {t} = this.props;

        return (
            <Input
                icon="search"
                className="search-input"
                onKeyDown={this.handleKeyPress}
                onChange={this.handleChange}
                placeholder={t('search_all_fields')}
                value={this.state.value}
            />
        );
    }
}

export default withTranslation()(Search);
