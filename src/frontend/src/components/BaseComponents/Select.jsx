import React, { useEffect } from 'react';
import { useTranslation } from 'react-i18next';
import { useDispatch, useSelector } from 'react-redux';

import { Dropdown, Form } from 'semantic-ui-react';

import './style.scss';
import { getLookupRequest, valuesListSelector } from '../../ducks/lookup';

const Select = ({
    value,
    onChange,
    placeholder = '',
    isDisabled,
    label,
    name,
    text,
    multiple,
    loading,
    clearable,
    source,
    isTranslate,
    error,
    textValue,
    errorText,
    noLabel,
}) => {
    const { t } = useTranslation();
    const dispatch = useDispatch();

    useEffect(() => {
        dispatch(
            getLookupRequest({
                name: source,
                isForm: true,
            }),
        );
    }, []);

    const valuesList = useSelector(state => valuesListSelector(state, source)) || [];

    const handleChange = (e, { value }) => {
        onChange(e, { value, name });
    };

    let items = valuesList.map((x, index) => ({
        key: `${x.value}_${index}`,
        value: x.value,
        text: isTranslate ? t(x.name) : x.name,
        /* disabled: !x.isActive,
        description: x.description,*/
    }));

    return (
        <Form.Field>
            {!noLabel ? (
                <label className={isDisabled ? 'label-disabled' : null}>{t(text || name)}</label>
            ) : null}
            <Dropdown
                placeholder={placeholder}
                fluid
                clearable={clearable}
                selection
                loading={loading}
                search
                text={textValue}
                error={error}
                multiple={multiple}
                disabled={isDisabled}
                value={value}
                options={items}
                onChange={handleChange}
                selectOnBlur={false}
            />
            {errorText && <span className="label-error">{errorText}</span>}
        </Form.Field>
    );
};

export default Select;
