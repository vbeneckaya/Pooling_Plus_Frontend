import React, { Component } from 'react';
import { connect } from 'react-redux';

import { Button, Icon, Form, Dropdown } from 'semantic-ui-react';

import './style.scss';

class Select extends Component {
    handleChange = (e, { value }) => {
        const { name, onChange } = this.props;
        onChange(e, { value, name });
    };

    render() {
        const { value, onChange, valuesList = [], placeholder = '', disabled, label, multiple, loading, clearable } = this.props;

        let items = valuesList.map((x, index) => ({
            key: `${x.value}_${index}`,
            value: x.value,
            text: x.name,
            disabled: !x.isActive,
            description: x.description,
        }));

        return (
            <Form.Field>
                {label ? <label>{label}</label> : null}
                <Dropdown
                    placeholder={placeholder}
                    fluid
                    clearable={clearable}
                    selection
                    loading={loading}
                    search
                    multiple={multiple}
                    disabled={disabled}
                    value={value}
                    options={items}
                    onChange={this.handleChange}
                />
            </Form.Field>
        );
    }
}

export default Select;
