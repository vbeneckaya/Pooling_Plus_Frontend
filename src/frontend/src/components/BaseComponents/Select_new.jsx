import React, {useEffect} from 'react';
import {useTranslation} from 'react-i18next';
import {useDispatch, useSelector} from 'react-redux';

import {Dropdown, Form} from 'semantic-ui-react';

import './style.scss';
import {getLookupRequest, valuesListSelector} from '../../ducks/lookup';

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
                    noLabel,
                    isRequired,
                    autoComplete,
                    children,
                }) => {
    const {t} = useTranslation();
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

    const handleChange = (e, {value}) => {
        onChange(e, {value, name, ext: valuesList.find(x => x.value === value)});
    };

    let items =
        valuesList &&
        valuesList.map((x, index) => ({
            key: `${x.value}_${index}`,
            value: x.value,
            text: isTranslate ? t(x.name) : x.name,
        }));

    console.log('select');

    return (
        <Form.Field>
            {!noLabel ? (
                <label className={isDisabled ? 'label-disabled' : null}>{`${t(text || name)}${
                    isRequired ? ' *' : ''
                    }`}</label>
            ) : null}
            <div className="form-select">
                <Dropdown
                    placeholder={placeholder}
                    fluid
                    clearable={clearable}
                    selection
                    loading={loading}
                    search
                    text={textValue}
                    error={error}
                    disabled={isDisabled}
                    value={value}
                    options={items}
                    onChange={handleChange}
                    selectOnBlur={false}
                    autoComplete={autoComplete}
                >
                    <Dropdown.Menu>
                        {items.map(item => (
                            <Dropdown.Item
                                key={item.key}
                                selected={item.value === value}
                                onClick={e => handleChange(e, {name, value: item.value})}
                            >
                                {item.text}
                            </Dropdown.Item>
                        ))}
                    </Dropdown.Menu>
                </Dropdown>
                {children && children}
            </div>
            {error && typeof error === 'string' && <span className="label-error">{error}</span>}
        </Form.Field>
    );
};

export default React.memo(Select);
