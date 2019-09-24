import React, { Component } from 'react';
import { useTranslation } from 'react-i18next';

import { Button, Icon, Form, Dropdown } from 'semantic-ui-react';

import './style.scss';

const Select = ({
    value,
    onChange,
    valuesList = [],
    placeholder = '',
    disabled,
    label,
    name,
    multiple,
    loading,
    clearable,
}) => {
    const { t } = useTranslation();
    const handleChange = (e, { value }) => {
        onChange(e, { value, name });
    };

    let items = valuesList.map((x, index) => ({
        key: `${x.value}_${index}`,
        value: x.value,
        text: x.name,
        disabled: !x.isActive,
        description: x.description,
    }));

    return (
        <Form.Field>
            <label>{t(name)}</label>
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
                onChange={handleChange}
            />
        </Form.Field>
    );
};

export default Select;
