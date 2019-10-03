import React, { useEffect } from 'react';
import { useTranslation } from 'react-i18next';
import { useSelector, useDispatch } from 'react-redux';

import { Button, Icon, Form, Dropdown } from 'semantic-ui-react';

import './style.scss';
import {getLookupRequest, valuesListSelector} from "../../ducks/lookup";

const Select = ({
    value,
    onChange,
    placeholder = '',
    disabled,
    label,
    name,
    text,
    multiple,
    loading,
    clearable,
    source
}) => {
    const { t } = useTranslation();
    const dispatch = useDispatch();

    useEffect(() => {
        dispatch(getLookupRequest({
            name: source,
            isForm: true,
        }))
    }, []);

    const valuesList = useSelector(state => valuesListSelector(state, source)) || [];

    const handleChange = (e, { value }) => {
        onChange(e, { value, name });
    };

    let items = valuesList.map((x, index) => ({
        key: `${x.value}_${index}`,
        value: x.value,
        text: x.name,
       /* disabled: !x.isActive,
        description: x.description,*/
    }));

    return (
        <Form.Field>
            {name ? <label>{t(text || name)}</label> : null}
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
